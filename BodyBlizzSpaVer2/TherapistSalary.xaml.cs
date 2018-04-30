using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for TherapistSalary.xaml
    /// </summary>
    public partial class TherapistSalary : MetroWindow
    {
        public TherapistSalary()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        TherapistModel therapist;
        List<TherapistModel> lstTherapist = new List<TherapistModel>();
        List<ServiceMadeModel> lstServiceMade = new List<ServiceMadeModel>();
        double dblCashAdvance = 0.0;
        double dblLoans = 0.0;

        public TherapistSalary(TherapistModel tm)
        {
            therapist = tm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (therapist != null)
            {
                lblTherapistName.Content = therapist.Description;
            }
        }

        public void searchTherapist()
        {
            lstServiceMade = new List<ServiceMadeModel>();
            ServiceMadeModel serviceMade = new ServiceMadeModel();
            if (therapist != null)
            {
               
                try
                {
                    string queryString = "SELECT tblServiceMade.ID, dbspa.tblservicemade.dateServiced as 'DATE SERVICED'," +
                        "dbspa.tbltherapist.description as 'THERAPIST', dbspa.tblservicetype.serviceType as 'SERVICE TYPE'," +
                        " dbspa.tblcommissions.commission, dbspa.tblservicemade.isDiscounted, dbspa.tbldiscount.discount, ifPaid, " +
                        "concat(dbspa.tblclient.firstName, ' ', dbspa.tblclient.lastName) AS CLIENT_NAME FROM (((((dbspa.tblservicemade " +
                        "INNER JOIN dbspa.tbltherapist ON dbspa.tblservicemade.therapistID = dbspa.tblTherapist.ID) " +
                        "INNER JOIN dbspa.tblservicetype ON dbspa.tblservicemade.serviceTypeID = dbspa.tblservicetype.ID) " +
                        "INNER JOIN dbspa.tblcommissions ON dbspa.tblservicemade.commissionID = dbspa.tblcommissions.ID) " +
                        "INNER JOIN dbspa.tbldiscount ON dbspa.tblservicemade.discountID = dbspa.tbldiscount.ID) " +
                        "INNER JOIN dbspa.tblclient ON dbspa.tblservicemade.clientID = dbspa.tblclient.ID) " +
                        "WHERE(dbspa.tbltherapist.ID = ?) AND (dbspa.tblservicemade.dateServiced BETWEEN ? AND ?) AND " +
                        "(dbspa.tblservicemade.isDeleted = 0) AND (dbspa.tblclient.isDeleted = 0) AND (dbspa.tblservicetype.isDeleted = 0)";

                    List<string> parameters = new List<string>();
                    parameters.Add(therapist.ID1);
                    DateTime date = DateTime.Parse(dateFrom.Text);
                    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                    date = DateTime.Parse(dateTo.Text);
                    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
                    int i = 0;
                    double total = 0.0;

                    
                    while (reader.Read())
                    {
                        serviceMade.ID = reader["ID"].ToString();
                        DateTime dte = DateTime.Parse(reader["DATE SERVICED"].ToString());
                        serviceMade.DateServiced = dte.ToShortDateString();
                        serviceMade.Therapist = reader["THERAPIST"].ToString();
                        serviceMade.ServiceType = reader["SERVICE TYPE"].ToString();
                        serviceMade.Commission = reader["commission"].ToString();
                        string strPaid = reader["ifPaid"].ToString();
                        serviceMade.ClientName = reader["CLIENT_NAME"].ToString();
                        if (strPaid.Equals("1"))
                        {
                            serviceMade.ifPaid = true;
                        }else
                        {
                            serviceMade.ifPaid = false;
                        }

                        serviceMade.ifPromoService = false;
                        serviceMade.isDiscounted = reader["isDiscounted"].ToString();
                        serviceMade.Discount = reader["discount"].ToString();

                        if (serviceMade.isDiscounted.Equals("1"))
                        {
                            double a = Convert.ToInt32(serviceMade.Commission);
                            double b = (Convert.ToDouble(serviceMade.Discount) / 100);
                            double dsc = a * b;

                            serviceMade.Commission = (Convert.ToInt32(serviceMade.Commission) - dsc).ToString();
                        }

                        lstServiceMade.Add(serviceMade);
                        
                        serviceMade = new ServiceMadeModel();
                    }

                    conDB.closeConnection();
                    lstServiceMade.AddRange(searchServicesRenderedInPromoServices(getAllPromoIDForTherapist()));
                    lstServiceMade = lstServiceMade.OrderByDescending(srv => srv.DateServiced).ToList();
                    dgvSalary.ItemsSource = lstServiceMade;

                    lblRecordsFound.Content = lstServiceMade.Count + " RECORD/S FOUND.";

                    foreach (ServiceMadeModel smm in lstServiceMade)
                    {
                        if (!smm.ifPaid)
                        {
                            double price = Convert.ToDouble(smm.Commission);
                            total += price;
                        }

                        i++;
                    }
                    lblTotal.Content = "TOTAL COMMISSION: " + total.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR " + ex.Message);
                }

            }
        }

        private List<ServiceMadeModel> searchServicesRenderedInPromoServices(List<string> lst)
        {
            List<ServiceMadeModel> lstPromoServicesRendered = new List<ServiceMadeModel>();
            ServiceMadeModel renderedServices = new ServiceMadeModel();

            foreach(string strPromoID in lst)
            {
                string queryString = "SELECT dbspa.tblpromoservicemade.ID, dateserviced, dbspa.tbltherapist.description, " +
                "dbspa.tblpromo.promoname, (truncate(dbspa.tblpromo.commission / numOfServ, 2)) AS 'commission' FROM (SELECT COUNT(*) as numOfServ " +
                "FROM dbspa.tblpromoservices where promoID = ? AND isDeleted = 0) AS tbl1, ((dbspa.tblpromoservicemade INNER JOIN " +
                "dbspa.tbltherapist ON dbspa.tblpromoservicemade.therapistID = dbspa.tbltherapist.ID) INNER JOIN " +
                "dbspa.tblpromo ON dbspa.tblpromoservicemade.promoID = dbspa.tblpromo.ID) WHERE dbspa.tblpromoservicemade.isDeleted = 0" +
                " AND (dbspa.tblpromoservicemade.dateserviced BETWEEN ? AND ?) AND (dbspa.tblpromoservicemade.therapistID = ?)";

                List<string> parameters = new List<string>();
                parameters.Add(strPromoID);
                DateTime date = DateTime.Parse(dateFrom.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                date = DateTime.Parse(dateTo.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                parameters.Add(therapist.ID1);

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    renderedServices.ifPromoService = true;
                    renderedServices.ID = reader["ID"].ToString();

                    DateTime dte = DateTime.Parse(reader["dateserviced"].ToString());
                    renderedServices.DateServiced = dte.ToShortDateString();

                    renderedServices.Therapist = reader["description"].ToString();
                    renderedServices.ServiceType = reader["promoname"].ToString();
                    renderedServices.Commission = reader["commission"].ToString();
                    lstPromoServicesRendered.Add(renderedServices);
                    renderedServices = new ServiceMadeModel();

                }
            }

            

            conDB.closeConnection();

            return lstPromoServicesRendered;
        }

        private List<string> getAllPromoIDForTherapist()
        {
            List<string> lstPromoIDs = new List<string>();
            string p = "";
            string queryString = "SELECT promoID FROM dbspa.tblpromoservicemade WHERE therapistID = ? AND isDeleted = 0 GROUP BY promoID";
            List<string> parameters = new List<string>();
            parameters.Add(therapist.ID1);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            while (reader.Read())
            {
                p = "";
                p = reader["promoID"].ToString();
                lstPromoIDs.Add(p);
               
            }
            conDB.closeConnection();

            return lstPromoIDs;
        }

        private void getAttendanceList()
        {
            AttendanceModel attendance = new AttendanceModel();
            List<AttendanceModel> lstAttendance = new List<AttendanceModel>();
            if (therapist != null)
            {
                try
                {
                    string queryString = "SELECT ID, attendanceDate AS 'DATE', timeIn AS 'TIME IN', timeOut AS 'TIME OUT', minutesLate AS 'MINS LATE', " +
                        "deduction as 'DEDUCTION', IF(isLate,'YES','NO') AS 'LATE', IF(ifhalfday,'YES','NO') AS 'HALFDAY', IF(ifUndertime,'YES','NO') " +
                        "AS 'UNDERTIME' FROM dbspa.tblattendance WHERE (therapistID = ?) AND (attendanceDate BETWEEN ? AND ?) AND (isDeleted = 0)";

                    List<string> parameters = new List<string>();
                    parameters.Add(therapist.ID1);

                    DateTime date = DateTime.Parse(dateFrom.Text);
                    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                    date = DateTime.Parse(dateTo.Text);
                    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                    while(reader.Read())
                    {
                        attendance.ID = reader["ID"].ToString();
                        DateTime dte = DateTime.Parse(reader["DATE"].ToString());
                        attendance.Date = dte.ToShortDateString();
                        attendance.TimeIn = reader["TIME IN"].ToString();
                        attendance.TimeOut = reader["TIME OUT"].ToString();
                        attendance.MinutesLate = reader["MINS LATE"].ToString();
                        attendance.Deduction = reader["DEDUCTION"].ToString();
                        attendance.Late = reader["LATE"].ToString();
                        attendance.HalfDay = reader["HALFDAY"].ToString();
                        attendance.Undertime = reader["UNDERTIME"].ToString();
                        lstAttendance.Add(attendance);
                        attendance = new AttendanceModel();
                    }

                    //dataTable = conDB.fillDataGridView(queryString, parameters);
                    conDB.closeConnection();
                    dgvAttendanceList.ItemsSource = lstAttendance;
                    
                    lblRecordAtt.Content = lstAttendance.Count.ToString() + " RECORD/S FOUND.";
                    //command.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR " + ex.Message);
                }
            }
        }

        private double getCashAdvance()
        {
            double dblCA = 0.0;

            string queryString = "SELECT SUM(dbspa.tblcashadvance.cash) as ca FROM dbspa.tblcashadvance WHERE therapistID = ? AND isDeleted = 0 " +
                " AND (Date BETWEEN ? AND ?)";
            List<string> parameters = new List<string>();

            parameters.Add(therapist.ID1);

            DateTime date = DateTime.Parse(dateFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(dateTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            string strCA = "";
            while (reader.Read())
            {
                strCA = reader["ca"].ToString();
                dblCA = !string.IsNullOrEmpty(strCA) ? Convert.ToDouble(strCA) : Convert.ToDouble("0");
            }

            conDB.closeConnection();

            return dblCA;
        }

        private double getLoans()
        {
            double dblLoansToPay = 0.0;

            string queryString = "SELECT SUM(dbspa.tblloanbalance.amount) as loan FROM dbspa.tblloanbalance WHERE therapistID = ? and isDeleted = 0 " +
                "AND (datepaid BETWEEN ? AND ?)";

            List<string> parameters = new List<string>();
            parameters.Add(therapist.ID1);

            DateTime date = DateTime.Parse(dateFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(dateTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
            string strLoansToPay = "";
            while (reader.Read())
            {
                strLoansToPay = reader["loan"].ToString();
                dblLoansToPay = !string.IsNullOrEmpty(strLoansToPay) ? Convert.ToDouble(strLoansToPay) : Convert.ToDouble("0");
                
            }
            conDB.closeConnection();


            return dblLoansToPay;   
        }

        private List<TherapistModel> getAttendanceForTherapist(int ID)
        {
            TherapistModel thera = new TherapistModel();
            List<TherapistModel> lstThera = new List<TherapistModel>();
            try
            {

                string queryString = "SELECT ID, attendanceDate, timeIn, timeOut, therapistID, minutesLate, isLate, deduction, ifhalfday, " +
                    "ifUndertime, lateDeduction, undertimeDeduction FROM dbspa.tblattendance WHERE  " +
                    "(dbspa.tblattendance.isDeleted = 0) AND (therapistID = ?) AND (attendanceDate BETWEEN ? AND ?)";
                List<string> parameters = new List<string>();

                parameters.Add(ID.ToString());

                DateTime date = DateTime.Parse(dateFrom.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                date = DateTime.Parse(dateTo.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    thera = new TherapistModel();
                    thera.ID1 = reader["therapistID"].ToString();
                    thera.AttendanceDate = DateTime.Parse(reader["attendanceDate"].ToString());
                    thera.TimeIn = reader["timeIn"].ToString();
                    thera.TimeOut = reader["timeOut"].ToString();
                    thera.MinutesLate = reader["minutesLate"].ToString();
                    thera.IsLate = Convert.ToInt32(reader["isLate"].ToString());
                    thera.Deduction = reader["deduction"].ToString();
                    thera.Ifhalfday = Convert.ToInt32(reader["ifhalfday"].ToString());
                    thera.ifUnderTime = Convert.ToInt32(reader["ifUndertime"].ToString());
                    thera.lateDeduction = reader["lateDeduction"].ToString();
                    thera.undertimeDeduction = reader["undertimeDeduction"].ToString();
                    lstThera.Add(thera);

                }

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return lstThera;
        }

        private void compute(List<ServiceMadeModel> lstServicesMade)
        {
            string[] services;
            string[] amount;
            double total = 0.0;
            int i = 0;
            
            if (lstServicesMade.Count > 0)
            {
                services = new string[lstServicesMade.Count];
                amount = new string[lstServicesMade.Count];
            }
            else
            {
                services = new string[1];
                amount = new string[1];

                services[0] = "\r" + "NO SERVICE/S RENDERED" + "\n";
                amount[0] = "\r" + "0.00" + "\n";
            }

            foreach (ServiceMadeModel smm in lstServicesMade)
            {
                if (!smm.ifPaid)
                {
                    double price = Convert.ToDouble(smm.Commission);
                    services[i] = "\r" + smm.ServiceType + "\n";
                    amount[i] = "\r" + String.Format("{0:0.00}", price.ToString()) + "\n";
                    total += price;
                }

                i++;
            }

            therapist.Services = services;
            therapist.Amount = amount;
            therapist.ClientName = getClientNames(lstServiceMade);
            therapist.TotalCommission = String.Format("{0:0.00}", total.ToString());
            
        }

        private int getTotalDeduction(List<TherapistModel> lstTherapistModel)
        {
            int deduction = 0;
            foreach (TherapistModel tm in lstTherapistModel)
            {
                deduction = Convert.ToInt32(tm.Deduction) + Convert.ToInt32(tm.lateDeduction) + Convert.ToInt32(tm.undertimeDeduction);
            }

            return deduction;
        }

        private string[] getPenalties(List<TherapistModel> lstTherapistModel)
        {
            string[] penalties = new string[lstTherapistModel.Count];
            int i = 0;
            foreach (TherapistModel tm in lstTherapistModel)
            {
                string str = "\r" + " " + tm.AttendanceDate.ToShortDateString();
                if (tm.Ifhalfday > 0)
                {
                    str = str + "  HALF DAY - " + tm.Deduction + "| ";
                
                }
                
                if (tm.IsLate > 0)
                {
                    str = str + " LATE - " + tm.MinutesLate + " Minute/s - " +
                        "PHP" + tm.lateDeduction + "| ";
                }

                if (tm.ifUnderTime > 0)
                {
                     str = str + " UNDERTIME - " + tm.undertimeDeduction + "| ";
                }
                penalties[i] = str + "\n";
                i++;
            }

            return penalties;
        }

        private string[] getClientNames(List<ServiceMadeModel> lstSrvcMadeMdl)
        {
            string[] clientNames;
            if (lstSrvcMadeMdl.Count > 0)
            {
                clientNames = new string[lstSrvcMadeMdl.Count];
                int i = 0;
                foreach (ServiceMadeModel tm in lstSrvcMadeMdl)
                {
                    clientNames[i] = "\r" + tm.ClientName.ToString() + "\n";
                    i++;
                }

            }else
            {
                clientNames = new string[1];
                clientNames[0] = "\r" + "-------------" + "\n";
            }
            
            return clientNames;
        }

        private void updateRecordToPaid(bool blPaid, string recID)
        {
            string queryString = "UPDATE dbspa.tblservicemade SET ifPaid = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            if (blPaid)
            {
                parameters.Add("1");
            }else
            {
                parameters.Add("0");
            }
            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            if (checkDateFields())
            {
                DateTime dteTo = DateTime.Parse(dateTo.Text);
                DateTime dteFrm = DateTime.Parse(dateFrom.Text);

                int days = ((dteTo - dteFrm).Days) + 1;
                //double wage = Convert.ToDouble(therapist.Wage);
                searchTherapist();
                getAttendanceList();

                lstTherapist = getAttendanceForTherapist(Convert.ToInt32(therapist.ID1));
                //double total = lstTherapist.Count * wage;
                int deduct = getTotalDeduction(lstTherapist);
                //lblTotalWage.Content = "TOTAL WAGE: " + (total - deduct).ToString();
                therapist.NumberOfDays = days.ToString();
                //therapist.TotalWage = total.ToString();
                therapist.Penalties = getPenalties(lstTherapist);
                therapist.Deduction = deduct.ToString();
                chkCashAdvance.IsChecked = false;
                chkLoans.IsChecked = false;
                dblCashAdvance = getCashAdvance();
                dblLoans = getLoans();
            }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            List<ServiceMadeModel> newListServices = new List<ServiceMadeModel>();

            for (int i = 0; i < dgvSalary.Items.Count-1; i++)
            {              
                var item = dgvSalary.Items[i];
                var mycheckbox = dgvSalary.Columns[4].GetCellContent(item) as CheckBox;
                ServiceMadeModel smms = item as ServiceMadeModel;

                if(smms != null)
                {
                    if (!smms.ifPaid)
                    {
                        smms.ifPaid = false;
                        newListServices.Add(smms);
                    }
                }
            }

            compute(newListServices);
 
            ReportForm rf = new ReportForm(therapist, lstTherapist);
            rf.ShowDialog();
        }

        private bool checkDateFields()
        {
            bool allCorrect = false;

            if (string.IsNullOrEmpty(dateFrom.Text))
            {
                MessageBox.Show("Please select Date From");
            }
            else if (string.IsNullOrEmpty(dateTo.Text))
            {
                MessageBox.Show("Please select Date To");
            }
            else
            {
                allCorrect = true;
            }

            return allCorrect;
        }

        private void btnUpdateRecords_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgvSalary.Items.Count; i++)
            {
                progressBar.IsActive = true;
                var item = dgvSalary.Items[i];
                var mycheckbox = dgvSalary.Columns[4].GetCellContent(item) as CheckBox;
                ServiceMadeModel smms = item as ServiceMadeModel;
                if (smms != null && mycheckbox != null)
                {
                    updateRecordToPaid((bool)mycheckbox.IsChecked, smms.ID);
                    conDB.writeLogFile("COMMISSION SET TO PAID?: " + (bool)mycheckbox.IsChecked + " RECORD ID:" + smms.ID);
                }
            }
            MessageBox.Show("SELECTED RECORDS UPDATED SUCCESSFULLY!");
            progressBar.IsActive = false;
        }

        private void chkCashAdvance_Checked(object sender, RoutedEventArgs e)
        {
            therapist.CashAdvance = dblCashAdvance.ToString();
        }

        private void chkCashAdvance_Unchecked(object sender, RoutedEventArgs e)
        {
            therapist.CashAdvance = "0.0";
        }

        private void chkLoans_Checked(object sender, RoutedEventArgs e)
        {
            therapist.Loan = dblLoans.ToString();
        }

        private void chkLoans_Unchecked(object sender, RoutedEventArgs e)
        {
            therapist.Loan = "0.0";
        }
    }
}

using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for PrintExpensesWindow.xaml
    /// </summary>
    public partial class PrintExpensesWindow : MetroWindow
    {
        public PrintExpensesWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        List<CashAdvanceModel> lstCashAdvance = new List<CashAdvanceModel>();
        List<LoanModel> lstLoansPaid = new List<LoanModel>();
        string queryString = "";
        List<string> parameters;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private List<ServiceMadeModel> getAllServicesRenderedByDate()
        {
            List<ServiceMadeModel> lstServiceMade = new List<ServiceMadeModel>();
            ServiceMadeModel serviceMade = new ServiceMadeModel();

            queryString = "SELECT dbspa.tblservicemade.ID, dbspa.tblservicetype.servicetype AS 'SERVICE TYPE', " +
                "dbspa.tbltherapist.description AS 'THERAPIST', dbspa.tblservicemade.dateServiced, dbspa.tblcommissions.commission, " +
                "IF(dbspa.tblservicemade.isDiscounted, ROUND(dbspa.tblservicetype.price - dbspa.tblservicetype.price * (dbspa.tbldiscount.discount / 100)), " +
                "dbspa.tblservicetype.price) AS 'PRICE', IF(dbspa.tblservicemade.isDiscounted, 'YES', 'NO') AS 'DISCOUNTED' " +
                "FROM (((((dbspa.tblservicemade INNER JOIN dbspa.tblclient ON dbspa.tblservicemade.clientID = dbspa.tblClient.ID) " +
                "INNER JOIN dbspa.tblservicetype ON tblservicemade.serviceTypeID = dbspa.tblservicetype.ID) " +
                "INNER JOIN dbspa.tbltherapist ON dbspa.tblservicemade.therapistID = dbspa.tbltherapist.ID) " +
                "INNER JOIN dbspa.tbldiscount ON dbspa.tblservicemade.discountID = dbspa.tbldiscount.ID) " +
                "INNER JOIN dbspa.tblcommissions ON dbspa.tblservicemade.commissionID = dbspa.tblcommissions.ID) WHERE (dbspa.tblservicemade.isDeleted = 0)" +
                " AND (dbspa.tblclient.isDeleted = 0) AND (dbspa.tbltherapist.isDeleted = 0) AND (dbspa.tblservicemade.dateServiced BETWEEN ? AND ?)";

            parameters = new List<string>();
            DateTime date = DateTime.Parse(datePickerFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(datePickerTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                serviceMade.ID = reader["ID"].ToString();
                serviceMade.ServiceType = reader["SERVICE TYPE"].ToString();
                serviceMade.Therapist = reader["THERAPIST"].ToString();
                serviceMade.DateServiced = reader["dateServiced"].ToString();
                serviceMade.Price = reader["PRICE"].ToString();
                serviceMade.Commission = reader["commission"].ToString();
                serviceMade.Discounted = reader["DISCOUNTED"].ToString();
                lstServiceMade.Add(serviceMade);
                serviceMade = new ServiceMadeModel();
            }
            conDB.closeConnection();
            lstServiceMade.AddRange(getAllPromoServicesRenderedByDate());
            return lstServiceMade;
        }

        private List<ServiceMadeModel> getAllPromoServicesRenderedByDate()
        {
            List<ServiceMadeModel> lstServiceMade = new List<ServiceMadeModel>();
            ServiceMadeModel serviceMade = new ServiceMadeModel();

            queryString = "SELECT dbspa.tblpromoservicesclient.ID, dbspa.tblpromo.commission, promoID, promoprice, " +
                "loyaltyID, dbspa.tblpromo.promoname,dbspa.tblpromoservicesclient.dateserviced, IF(dbspa.tblpromoservicesclient.savetocard, 'YES', 'NO')" +
                " AS 'SAVED TO CARD' FROM (dbspa.tblpromoservicesclient INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservicesclient.promoID = dbspa.tblpromo.ID)" +
                " WHERE dateserviced BETWEEN ? AND ? ";

            parameters = new List<string>();

            DateTime date = DateTime.Parse(datePickerFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(datePickerTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                serviceMade.ID = reader["ID"].ToString();
                serviceMade.ServiceType = reader["promoname"].ToString();
                serviceMade.Price = reader["promoprice"].ToString();
                lstServiceMade.Add(serviceMade);
                serviceMade = new ServiceMadeModel();
            }

            conDB.closeConnection();
            return lstServiceMade;
        }

        private List<CashAdvanceModel> getAllCashAdvanceByDate()
        {
            lstCashAdvance = new List<CashAdvanceModel>();
            CashAdvanceModel cashAdvanceModel = new CashAdvanceModel();

            queryString = "SELECT ID, therapistID, cash FROM dbspa.tblcashadvance WHERE " +
                "(Date BETWEEN ? AND ?) AND isDeleted = 0";

            parameters = new List<string>();
            DateTime date = DateTime.Parse(datePickerFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(datePickerTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                cashAdvanceModel.ID = reader["ID"].ToString();
                cashAdvanceModel.TherapistID = reader["therapistID"].ToString();
                cashAdvanceModel.Cash = reader["cash"].ToString();
                lstCashAdvance.Add(cashAdvanceModel);
                cashAdvanceModel = new CashAdvanceModel();              
            }

            conDB.closeConnection();
            return lstCashAdvance;
        }

        private List<LoanModel> getLoansPaidByDate()
        {
            lstLoansPaid = new List<LoanModel>();
            LoanModel lo = new LoanModel();

            queryString = "SELECT amount FROM dbspa.tblloanbalance WHERE " +
                "(datepaid BETWEEN ? AND ?) AND isDeleted = 0";

            parameters = new List<string>();
            DateTime date = DateTime.Parse(datePickerFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(datePickerTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                lo.LoanBalance = reader["amount"].ToString();
                lstLoansPaid.Add(lo);
                lo = new LoanModel();
            }

            conDB.closeConnection();

            return lstLoansPaid;
        }

        private List<ExpensesModel> getAllExpenses()
        {
            List<ExpensesModel> lstExpenses = new List<ExpensesModel>();
            ExpensesModel expenses = new ExpensesModel();

            queryString = "SELECT ID, date, description, cashout FROM dbspa.tblexpenses WHERE isDeleted = 0 AND (date BETWEEN ? AND ?)";

            parameters = new List<string>();
            DateTime date = DateTime.Parse(datePickerFrom.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            date = DateTime.Parse(datePickerTo.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                expenses.ID = reader["ID"].ToString();
                expenses.Date = reader["date"].ToString();
                expenses.Description = reader["description"].ToString();
                expenses.CashOut = reader["cashout"].ToString();
                lstExpenses.Add(expenses);
                expenses = new ExpensesModel();
            }

            conDB.closeConnection();
            
            return lstExpenses;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                DateTime dteFrom = DateTime.Parse(datePickerFrom.Text);
                DateTime dteTo = DateTime.Parse(datePickerTo.Text);

                ReportForm rf = new ReportForm(getAllServicesRenderedByDate(), getAllExpenses(), lstCashAdvance, lstLoansPaid, (dteFrom.ToShortDateString() + " - " + dteTo.ToShortDateString()));
                rf.ShowDialog();
            }
           
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                lstCashAdvance = getAllCashAdvanceByDate();
            }else
            {
                checkBox.IsChecked = false;
            }
            
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            lstCashAdvance = new List<CashAdvanceModel>();
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                lstLoansPaid = getLoansPaidByDate();
            }else
            {
                checkBox1.IsChecked = false;
            }
            
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            lstLoansPaid = new List<LoanModel>();
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(datePickerFrom.Text))
            {
                MessageBox.Show("Please select 'From Date'");
            }else if (string.IsNullOrEmpty(datePickerTo.Text))
            {
                MessageBox.Show("Please select 'To Date'");
            }else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

    }
}

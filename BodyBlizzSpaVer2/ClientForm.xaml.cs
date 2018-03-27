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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ClientForm.xaml
    /// </summary>
    public partial class ClientForm : MetroWindow
    {
        public ClientForm()
        {
            InitializeComponent();
        }

       
        ConnectionDB conDB = new ConnectionDB();
        User user;
        public ClientForm(User usr)
        {
            user = usr;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dateFrom.Text = DateTime.Now.ToShortDateString();
            if(user.Type > 1)
            {
                btnDelete.IsEnabled = false;
            }
            dgvClient.ItemsSource = getClientInfo();
            

        }

        private List<ClientModel> getClientInfo()
        {
            List<ClientModel> lstClient = new List<ClientModel>();
            ClientModel cm = new ClientModel();
            try
            {
                string queryString = "SELECT dbspa.tblclient.ID AS 'ID',dbspa.tblclient.firstName AS 'FIRST NAME'," + 
                    "dbspa.tblclient.lastName AS 'LAST NAME', dbspa.tblservicemode.serviceType AS 'SERVICE MODE', " +
                    "IF(dbspa.tblclient.isLoyal, 'YES', 'NO') AS 'LOYALTY CARD', LoyaltyID, dbspa.tblclient.phonenumber FROM (dbspa.tblclient INNER JOIN dbspa.tblservicemode ON " +
                    "dbspa.tblclient.servicemode = dbspa.tblservicemode.ID) WHERE (dbspa.tblclient.isDeleted = 0) order by ID desc";

                List<string> parameters = new List<string>();
                DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
                while (reader.Read())
                {
                    cm.ID1 = reader["ID"].ToString();
                    cm.FirstName = reader["FIRST NAME"].ToString();
                    cm.LastName = reader["LAST NAME"].ToString();
                    cm.PhoneNumber = reader["phonenumber"].ToString();
                    cm.ServiceMode = reader["SERVICE MODE"].ToString();
                    cm.isLoyal = reader["LOYALTY CARD"].ToString();
                    cm.LoyaltyID = reader["LoyaltyID"].ToString();
                    lstClient.Add(cm);
                    cm = new ClientModel();
                }
                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            return lstClient;
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            if(checkDateFields())
            {

                List<ClientModel> lstClientModel = new List<ClientModel>();
                ClientModel cm = new ClientModel();
                string queryString = "SELECT dbspa.tblclient.ID AS 'ID',dbspa.tblclient.firstName AS 'FIRST NAME'," +
                        "dbspa.tblclient.lastName AS 'LAST NAME', dbspa.tblservicemode.serviceType AS 'SERVICE MODE', " +
                        "IF(dbspa.tblclient.isLoyal, 'YES', 'NO') AS 'LOYALTY CARD', LoyaltyID, dbspa.tblclient.phonenumber FROM (dbspa.tblclient INNER JOIN dbspa.tblservicemode ON " +
                        "dbspa.tblclient.servicemode = dbspa.tblservicemode.ID)  WHERE (dbspa.tblclient.isDeleted = 0) ";

                List<string> parameters = new List<string>();

                if(chkDate.IsChecked == true)
                {
                    queryString += " AND (dbspa.tblclient.dateServiced = ?)";
                    DateTime sdate = DateTime.Parse(dateFrom.Text);
                    parameters.Add(sdate.Year + "/" + sdate.Month + "/" + sdate.Day);
                }
                if(chkLastname.IsChecked == true)
                {
                    queryString += " AND (dbspa.tblclient.lastName LIKE '%" +txtLastName.Text+ "%')";
                }
                if(chkFirstname.IsChecked == true)
                {
                    queryString += " AND (dbspa.tblclient.firstName LIKE '%" + txtFirstName.Text + "%')";
                }

                queryString += "ORDER BY ID desc";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    cm.ID1 = reader["ID"].ToString();
                    cm.FirstName = reader["FIRST NAME"].ToString();
                    cm.LastName = reader["LAST NAME"].ToString();
                    cm.PhoneNumber = reader["phonenumber"].ToString();
                    cm.ServiceMode = reader["SERVICE MODE"].ToString();
                    cm.isLoyal = reader["LOYALTY CARD"].ToString();
                    cm.LoyaltyID = reader["LoyaltyID"].ToString();
                    lstClientModel.Add(cm);
                    cm = new ClientModel();
                }

                dgvClient.ItemsSource = lstClientModel;
                conDB.closeConnection();
            }

            
        }

        private ClientModel getClientDetails(int id)
        {
            ClientModel cm = new ClientModel();

            string queryString = "SELECT dateServiced, serialNo, cod, firstName, lastName, address, phonenumber, servicemode, totalamt, " +
            "timeIn, timeOut, isLoyal, LoyaltyID FROM dbspa.tblclient WHERE (ID = ?) AND (isDeleted = 0)";

            List<string> parameters = new List<string>();

            parameters.Add(id.ToString());
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                cm.DateServiced = reader["dateServiced"].ToString();
                cm.SerialNumber = reader["serialNo"].ToString();
                cm.Cod = reader["cod"].ToString();
                cm.FirstName = reader["firstName"].ToString();
                cm.LastName = reader["lastName"].ToString();
                cm.Address = reader["address"].ToString();
                cm.PhoneNumber = reader["phonenumber"].ToString();
                cm.ServiceMode = reader["servicemode"].ToString();
                cm.TotalAmt = reader["totalamt"].ToString();
                cm.TimeIn = reader["timeIn"].ToString();
                cm.TimeOut = reader["timeOut"].ToString();
                cm.isLoyal = reader["isLoyal"].ToString();
                cm.LoyaltyID = reader["LoyaltyID"].ToString();
            }

            conDB.closeConnection();

            return cm;
        }

        private LoyaltyCardModel getServiceCountForLoyaltyOwner(string cardID)
        {
            LoyaltyCardModel lcm = new LoyaltyCardModel();

            string queryString = "SELECT servicecount, firstfree, secondfree FROM dbspa.tblloyaltycard WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(cardID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                lcm.ServiceCount = Convert.ToInt32(reader["servicecount"].ToString());
                lcm.FirstFree = reader["firstfree"].ToString();
                lcm.SecondFree = reader["secondfree"].ToString();


            }


            return lcm;
        }

        private List<ServiceMadeModel> getServiceMade(int id)
        {
            List<ServiceMadeModel> lstSMM = new List<ServiceMadeModel>();
            ServiceMadeModel smm = new ServiceMadeModel();

            string queryString = "SELECT dbspa.tblservicemade.ID, serviceTypeID, therapistID, dateServiced, isDiscounted, discountID, " +
                "dbspa.tblservicetype.price, dbspa.tblservicetype.serviceType FROM (dbspa.tblservicemade INNER JOIN dbspa.tblservicetype ON dbspa.tblservicemade.serviceTypeID = dbspa.tblservicetype.ID) " +
                "WHERE(clientID = ?) AND (dbspa.tblservicemade.isDeleted = 0) AND dateServiced = ?";
            List<string> parameters = new List<string>();

            parameters.Add(id.ToString());
            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                smm.ID = reader["ID"].ToString();
                smm.ServiceTypeID = reader["serviceTypeID"].ToString();
                smm.ServiceType = reader["serviceType"].ToString();
                smm.Therapist = reader["therapistID"].ToString();
                smm.isDiscounted = reader["isDiscounted"].ToString();
                smm.Discounted = reader["discountID"].ToString();
                smm.Price = reader["price"].ToString();
                
                lstSMM.Add(smm);
                smm = new ServiceMadeModel();
            }

            conDB.closeConnection();

            return lstSMM;
        }

        private List<ServiceTypeModel> getServiceType(List<ServiceMadeModel> smm)
        {
            List<ServiceTypeModel> lstSTM = new List<ServiceTypeModel>();
            ServiceTypeModel tm = new ServiceTypeModel();

            foreach (ServiceMadeModel madeModel in smm)
            {
                string queryString = "SELECT ID, serviceType, price FROM dbspa.tblservicetype WHERE ID = ? AND isDeleted=0";
                List<string> parameters = new List<string>();
                parameters.Add(madeModel.ServiceType);
                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    tm.ID1 = reader["ID"].ToString();
                    tm.ServiceType = reader["serviceType"].ToString();
                    tm.Price = reader["price"].ToString();

                    lstSTM.Add(tm);

                    tm = new ServiceTypeModel();
                }
                conDB.closeConnection();
            }
            return lstSTM;
        }

        private List<ServiceMadeModel> getServicesFromPromo(ClientModel c)
        {
            List<ServiceMadeModel> lstPromoServices = new List<ServiceMadeModel>();
            ServiceMadeModel s = new ServiceMadeModel();

            string queryString = "SELECT dbspa.tblpromoservicesclient.ID, dbspa.tblpromo.promoname, dbspa.tblpromo.price FROM " +
                "(dbspa.tblpromoservicesclient INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservicesclient.promoID = dbspa.tblpromo.ID) " +
                "WHERE dbspa.tblpromoservicesclient.clientID = ? AND dateserviced = ?";

            List<string> parameters = new List<string>();
            parameters.Add(c.ID1);
            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                s.ID = reader["ID"].ToString();
                s.ServiceType = reader["promoname"].ToString();
                s.Price = reader["price"].ToString();
                lstPromoServices.Add(s);
                s = new ServiceMadeModel();
            }

            return lstPromoServices;
        }

        private DiscountModel getDiscountedRate(int id)
        {
            DiscountModel dm = new DiscountModel();

            string queryString = "SELECT discount, description FROM dbspa.tbldiscount WHERE (ID=?) And (isDeleted = 0)";
            List<string> parameters = new List<string>();

            parameters.Add(id.ToString());

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                dm.Discount = reader["discount"].ToString();
                dm.Description = reader["description"].ToString();
            }

            conDB.closeConnection();

            return dm;
        }

        private List<TherapistModel> getTherapistServed(int id)
        {

            List<TherapistModel> lstTM = new List<TherapistModel>();
            TherapistModel tm = new TherapistModel();

            string queryString = "SELECT dbspa.tbltherapist.ID, dbspa.tbltherapist.firstName, dbspa.tbltherapist.lastName, dbspa.tbltherapist.wage, " +
                "dbspa.tbltherapist.description FROM ((dbspa.tblservicemade INNER JOIN dbspa.tbltherapist ON " +
                "dbspa.tblservicemade.therapistID = dbspa.tbltherapist.ID) INNER JOIN dbspa.tblClient ON " +
                "dbspa.tblservicemade.clientID = dbspa.tblclient.ID) WHERE (dbspa.tblservicemade.clientID = ?) AND (dbspa.tblservicemade.dateServiced = ?)";

            List<string> parameters = new List<string>();

            parameters.Add(id.ToString());

            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                tm.ID1 = reader["ID"].ToString();
                tm.FirstName = reader["firstName"].ToString();
                tm.LastName = reader["lastName"].ToString();
                tm.Wage = reader["wage"].ToString();
                tm.Description = reader["description"].ToString();
                lstTM.Add(tm);
                tm = new TherapistModel();
            }

            conDB.closeConnection();

        
            return lstTM;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ClientModel cm = new ClientModel();
            cm.IfEditDetails = false;
            cm.IfViewDetails = false;
            ClientDetails clientDetails = new ClientDetails(this, cm, user);
            clientDetails.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ClientModel cm = dgvClient.SelectedItem as ClientModel;

            if (cm != null)
            {
                int id = Convert.ToInt32(cm.ID1);
                cm = getClientDetails(id);
                cm.ID1 = id.ToString();
                if (cm.isLoyal.Equals("1"))
                {
                    LoyaltyCardModel lcm = getServiceCountForLoyaltyOwner(cm.LoyaltyID);
                    cm.ServiceCount = lcm.ServiceCount.ToString();
                    cm.FirstFree = lcm.FirstFree;
                    cm.SecondFree = lcm.SecondFree;
                }
                cm.IfViewDetails = false;
                cm.IfEditDetails = true;
                ClientDetails cd = new ClientDetails(this, cm, user);
                cd.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("No record selected!");
            }
        }

        private void printWaiver()
        {
            ClientModel cm = dgvClient.SelectedItem as ClientModel;

            if (cm != null)
            {
                int id = Convert.ToInt32(cm.ID1);
                cm = getClientDetails(id);
                cm.ID1 = id.ToString();
                cm.IfViewDetails = false;
                cm.IfEditDetails = false;
                cm.IfPrintWaiver = true;
                cm.LstServiceTypeModel = getServiceType(getServiceMade(id));
                cm.LstTherapistModel = getTherapistServed(id);
                conDB.writeLogFile("GENERATE REPORT: WAIVER FORM FOR CLIENT: " + cm.ID1);
                ReportForm rf = new ReportForm(this, cm);
                rf.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("No record selected!");
            }

        }

        private void deleteClientRecord(int id)
        {

            try
            {
                string queryString = "UPDATE dbspa.tblclient SET isDeleted = ? WHERE ID = ?";
                List<string> parameters = new List<string>();

                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);

                getClientInfo(dateFrom.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void getClientInfo(string selectedDate)
        {
            List<ClientModel> lstClient = new List<ClientModel>();
            ClientModel cm = new ClientModel();
            try
            {
                string queryString = "SELECT dbspa.tblclient.ID AS 'ID',dbspa.tblclient.firstName AS 'FIRST NAME'," +
                    "dbspa.tblclient.lastName AS 'LAST NAME', dbspa.tblservicemode.serviceType AS 'SERVICE MODE', " +
                    "IF(dbspa.tblclient.isLoyal, 'YES', 'NO') AS 'LOYALTY CARD', LoyaltyID, dbspa.tblclient.phonenumber FROM (dbspa.tblclient INNER JOIN dbspa.tblservicemode ON " +
                    "dbspa.tblclient.servicemode = dbspa.tblservicemode.ID)  WHERE (dbspa.tblclient.dateServiced = ?) AND (dbspa.tblclient.isDeleted = 0)";

                List<string> parameters = new List<string>();
                DateTime date = DateTime.Parse(selectedDate);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
                while (reader.Read())
                {
                    cm.ID1 = reader["ID"].ToString();
                    cm.FirstName = reader["FIRST NAME"].ToString();
                    cm.LastName = reader["LAST NAME"].ToString();
                    cm.PhoneNumber = reader["phonenumber"].ToString();
                    cm.ServiceMode = reader["SERVICE MODE"].ToString();
                    cm.isLoyal = reader["LOYALTY CARD"].ToString();
                    cm.LoyaltyID = reader["LoyaltyID"].ToString();
                    lstClient.Add(cm);
                    cm = new ClientModel();
                }
                conDB.closeConnection();
                dgvClient.ItemsSource = lstClient;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            List<ServiceMadeModel> lstServiceMade = new List<ServiceMadeModel>();
            ClientModel cm = dgvClient.SelectedItem as ClientModel;

            int id = Convert.ToInt32(cm.ID1);
            cm = getClientDetails(id);
            cm.ID1 = id.ToString();
            cm.IfViewDetails = true;
            lstServiceMade = getServiceMade(id);
            cm.LstServiceTypeModel = getServiceType(lstServiceMade);
            ClientDetails cd = new ClientDetails(this, cm, user);
            cd.ShowDialog();
            
        }

        private List<ProductBoughtModel> getProductsBoughtForClient(int id)
        {
            List<ProductBoughtModel> lstProductsBought = new List<ProductBoughtModel>();
            ProductBoughtModel prodBought = new ProductBoughtModel();

            string queryString = "SELECT dbspa.tblproductbought.ID, dbspa.tblproducts.productName, " +
                "IF(dbspa.tblproductbought.isDiscounted, (dbspa.tblproducts.price - dbspa.tblproducts.price * (dbspa.tbldiscount.discount / 100))  * dbspa.tblproductbought.totalqty, " +
                "dbspa.tblproducts.price * dbspa.tblproductbought.totalqty) AS 'PRICE', IF(dbspa.tblproductbought.isDiscounted, 'YES', 'NO') AS 'DISCOUNTED'" +
                " FROM(((dbspa.tblproductbought INNER JOIN dbspa.tblclient ON dbspa.tblproductbought.clientID = dbspa.tblclient.ID) " +
                "INNER JOIN dbspa.tblproducts ON dbspa.tblproductbought.productID = dbspa.tblproducts.ID) " +
                "INNER JOIN dbspa.tbldiscount ON dbspa.tblproductbought.discountID = dbspa.tbldiscount.ID) " +
                "WHERE dbspa.tblproductbought.isDeleted = 0 AND dbspa.tblproductbought.clientID = ? AND datebought =?";

            List<string> parameters = new List<string>();
            parameters.Add(id.ToString());

            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                prodBought.ID = reader["ID"].ToString();
                prodBought.ProductName = reader["productName"].ToString();
                prodBought.ProductPrice = reader["PRICE"].ToString();
                prodBought.isDiscounted = reader["DISCOUNTED"].ToString();
                lstProductsBought.Add(prodBought);
                prodBought = new ProductBoughtModel();
            }

            conDB.closeConnection();

            return lstProductsBought;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
          
            List<ServiceMadeModel> serviceMade = new List<ServiceMadeModel>();
            ClientModel cm = dgvClient.SelectedItem as ClientModel;

            if(cm != null)
            {
                int id = Convert.ToInt32(cm.ID1);

                if (id != 0)
                {
                    string servMode = cm.ServiceMode;
                    cm = getClientDetails(id);
                    cm.ID1 = id.ToString();
                    cm.ServiceMode = servMode;
                    cm.IfViewDetails = false;
                    cm.IfEditDetails = false;
                    cm.IfPrintWaiver = false;

                    //cm.ServiceMode = row.Cells[3].Value.ToString();
                    serviceMade = getServiceMade(id);

                    //cm.LstServiceTypeModel = getServiceType(serviceMade);
                    //cm.LstServiceTypeModel.AddRange(getServicesFromPromo(cm));
                    serviceMade.AddRange(getServicesFromPromo(cm));
                    cm.lstProductsBought = getProductsBoughtForClient(id);

                    //DISCOUNTED LSTSERVICE TYPE
                    List<ServiceTypeModel> discountedServiceType = new List<ServiceTypeModel>();
                    ServiceTypeModel discServiceType = new ServiceTypeModel();

                    foreach (ServiceMadeModel smmm in serviceMade)
                    {
                        //discServiceType = stm;
                        DiscountModel dm = getDiscountedRate(Convert.ToInt32(smmm.Discounted));

                        double price = Convert.ToDouble(smmm.Price);
                        double discount = Convert.ToInt32(dm.Discount);
                        double discountedPrice = (price * (discount / 100));

                        price = price - discountedPrice;
                        discServiceType.Price = price.ToString();
                        discServiceType.ServiceType = smmm.ServiceType;
                        discountedServiceType.Add(discServiceType);
                        discServiceType = new ServiceTypeModel();
                    }

                    cm.LstServiceTypeModel = discountedServiceType;

                    cm.LstTherapistModel = getTherapistServed(id);
                    conDB.writeLogFile("GENERATE REPORT: CLIENT FORM FOR: " + cm.ID1);
                    
                    ReportForm rf = new ReportForm(this, cm);
                    rf.ShowDialog();

                }
            }
            else
            {
                System.Windows.MessageBox.Show("No record selected!");
            }

                   
        }

        private void btnPrint2_Click(object sender, RoutedEventArgs e)
        {
            printWaiver();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", MessageBoxButtons.YesNo);
            
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                ClientModel cm = dgvClient.SelectedItem as ClientModel;

                if(cm != null)
                {
                    int id = Convert.ToInt32(cm.ID1);

                    if (id != 0)
                    {
                        deleteClientRecord(id);
                        System.Windows.MessageBox.Show("Record deleted successfuly!");
                        conDB.writeLogFile("DELETED CLIENT RECORD: CLIENT ID: " + cm.ID1);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No record selected!");
                }           
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool checkDateFields()
        {
            bool allCorrect = false;

            if(string.IsNullOrEmpty(dateFrom.Text))
            {
                System.Windows.MessageBox.Show("Please select Date");
            }else
            {

                allCorrect = true;
            }

            return allCorrect;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            chkDate.IsChecked = false;
            chkFirstname.IsChecked = false;
            chkLastname.IsChecked = false;

            txtFirstName.Text = "";
            txtLastName.Text = "";

            if (checkDateFields())
            {

                List<ClientModel> lstClientModel = new List<ClientModel>();
                ClientModel cm = new ClientModel();
                string queryString = "SELECT dbspa.tblclient.ID AS 'ID',dbspa.tblclient.firstName AS 'FIRST NAME'," +
                        "dbspa.tblclient.lastName AS 'LAST NAME', dbspa.tblservicemode.serviceType AS 'SERVICE MODE', " +
                        "IF(dbspa.tblclient.isLoyal, 'YES', 'NO') AS 'LOYALTY CARD', LoyaltyID, dbspa.tblclient.phonenumber FROM (dbspa.tblclient INNER JOIN dbspa.tblservicemode ON " +
                        "dbspa.tblclient.servicemode = dbspa.tblservicemode.ID)  WHERE (dbspa.tblclient.isDeleted = 0) order by ID desc";

                List<string> parameters = new List<string>();

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    cm.ID1 = reader["ID"].ToString();
                    cm.FirstName = reader["FIRST NAME"].ToString();
                    cm.LastName = reader["LAST NAME"].ToString();
                    cm.PhoneNumber = reader["phonenumber"].ToString();
                    cm.ServiceMode = reader["SERVICE MODE"].ToString();
                    cm.isLoyal = reader["LOYALTY CARD"].ToString();
                    cm.LoyaltyID = reader["LoyaltyID"].ToString();
                    lstClientModel.Add(cm);
                    cm = new ClientModel();
                }

                dgvClient.ItemsSource = lstClientModel;
                conDB.closeConnection();
            }

        }
    }
}

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
    /// Interaction logic for ServiceForm.xaml
    /// </summary>
    public partial class ServiceForm : MetroWindow
    {
        public ServiceForm()
        {
            InitializeComponent();
        }

        ClientDetails clientDet;
        ClientModel clientModel;
        ClientForm cf;
        ServiceMadeModel serviceMade;
        ServiceMadeModel serviceMadeDetails;
        ConnectionDB conDB = new ConnectionDB();
        User user;

        public ServiceForm(ClientDetails cdv, ClientForm cf1, ClientModel cm, User usr)
        {
            cf = cf1;
            clientDet = cdv;
            clientModel = cm;
            user = usr;
            InitializeComponent();
        }

        public ServiceForm(ClientDetails cdv)
        {
            clientDet = cdv;
            InitializeComponent();
        }

        public ServiceForm(ClientDetails cdv, ServiceMadeModel servMade, ClientModel cm, User usr)
        {
            clientDet = cdv;
            serviceMade = servMade;
            clientModel = cm;
            user = usr;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            chkAvailToCard.Visibility = Visibility.Hidden;
            chkFirstFree.Visibility = Visibility.Hidden;
            chkSecondFree.Visibility = Visibility.Hidden;
            cmbPromoServices.IsEnabled = false;
            List<PromoServicesModel> lstPromoS = new List<PromoServicesModel>();
            if (serviceMade != null)
            {
                if (serviceMade.ifPromoService)
                {
                    lstPromoS = getPromoServicesRendered(serviceMade.ID);
                    dgvPromoServices.ItemsSource = lstPromoS;
                    chkAvailPromo.IsChecked = true;
                    chkAvailPromo.IsEnabled = false;
                }
                else
                {
                    serviceMadeDetails = getServiceDetails(Convert.ToInt32(serviceMade.ID));
                    chkAvailPromo.IsEnabled = false;
                }

                if (serviceMade.isSavedToCard.Equals("YES"))
                {
                    chkAvailToCard.IsChecked = true;
                }

                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
                
            }
            if(clientModel != null)
            {
                if (clientModel.isLoyal.Equals("1"))
                {
                    lblLoyaltyOwner.Content = "LOYALTY CARD OWNER: YES";
                    chkAvailToCard.Visibility = Visibility.Visible;
                }else
                {
                    lblLoyaltyOwner.Content = "LOYALTY CARD OWNER: NO";
                }

                //CHECK IF FIRST 30 MINS READY TO AVAIL
                if (Convert.ToInt32(clientModel.ServiceCount) >= 5 && !clientModel.FirstFree.Equals("1"))
                {
                    chkFirstFree.Visibility = Visibility.Visible;
                }
                if(Convert.ToInt32(clientModel.ServiceCount) >= 10 && !clientModel.SecondFree.Equals("1"))
                {
                    chkSecondFree.Visibility = Visibility.Visible;
                }
            }
            fillPromoName();
            fillListBoxServiceType(cmbServices);
            fillComboTherapist(cmbTherapist, lstPromoS);
            fillComboDiscount(cmbDiscount);

        }

        #region FILL CONTROLS
        public void fillListBoxServiceType(ComboBox combo)
        {
            try
            {
                string queryString = "SELECT ID, serviceType AS 'SERVICE TYPE', price AS 'PRICE', description "+
                    "FROM dbspa.tblservicetype WHERE (isDeleted = 0) order by description";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    ServiceTypeModel stm = new ServiceTypeModel();
                    stm.ServiceType = reader["SERVICE TYPE"].ToString();
                    stm.Price = reader["price"].ToString();
                    stm.ID1 = reader["ID"].ToString();
                    stm.Description = reader["description"].ToString();
                    combo.Items.Add(stm);
                    if (serviceMadeDetails != null)
                    {
                        if (stm.ID1.Equals(serviceMadeDetails.ServiceType))
                        {
                            combo.SelectedItem = stm;
                        }

                    }
                    
                }
                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void fillComboDiscount(ComboBox cmb)
        {
            try
            {
                string queryString = "SELECT ID,discount, description FROM dbspa.tbldiscount WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    DiscountModel dm = new DiscountModel();
                    dm.ID1 = reader["ID"].ToString();
                    dm.Discount = reader["discount"].ToString();
                    dm.Description = reader["description"].ToString();

                    cmb.Items.Add(dm);
                    if (serviceMade != null)
                    {
                        if (dm.ID1.Equals(serviceMade.Discount))
                        {
                            cmb.SelectedItem = dm;
                        }

                    }
                }

                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void fillComboTherapist(ComboBox cmb, List<PromoServicesModel> lst)
        {
            try
            {
                List<TherapistModel> lstTherapistsServ = new List<TherapistModel>();
                string queryString = "SELECT ID,firstName, lastName, wage, description FROM dbspa.tbltherapist WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    TherapistModel tm = new TherapistModel();
                    tm.ID1 = reader["ID"].ToString();
                    tm.FirstName = reader["firstName"].ToString();
                    tm.LastName = reader["lastName"].ToString();
                    tm.Wage = reader["wage"].ToString();
                    tm.Description = tm.FirstName + " " + tm.LastName;
                    lstTherapistsServ.Add(tm);

                    cmb.Items.Add(tm);
                    if (serviceMadeDetails != null)
                    {
                        if (tm.ID1.Equals(serviceMadeDetails.Therapist))
                        {
                            cmb.SelectedItem = tm;
                        }

                    }

                }

                cmbTherapistAvailable.ItemsSource = lstTherapistsServ;
                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void fillPromoName()
        {
            PromoModel promo = new PromoModel();
            string queryString = "SELECT dbspa.tblpromo.ID, promoname, price FROM dbspa.tblpromo WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                promo.ID = reader["ID"].ToString();
                promo.PromoName = reader["promoname"].ToString();
                promo.PromoPrice = Convert.ToDouble(reader["price"].ToString());
                cmbPromoServices.Items.Add(promo);

                if (serviceMade != null && !string.IsNullOrEmpty(serviceMade.PromoID))
                {
                    if (serviceMade.PromoID.Equals(promo.ID))
                    {
                        cmbPromoServices.SelectedItem = promo;
                    }
                }
                promo = new PromoModel();

            }
            conDB.closeConnection();
        }

        #endregion

        private ServiceMadeModel getServiceDetails(int id)
        {
            ServiceMadeModel serviceMadeModel = new ServiceMadeModel();

            string queryString = "SELECT serviceTypeID, therapistID, discountID FROM dbspa.tblservicemade WHERE ID = ?";
            List<string> parameters = new List<string>();

            parameters.Add(id.ToString());

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                serviceMadeModel.ServiceType = reader["serviceTypeID"].ToString();
                serviceMadeModel.Therapist = reader["therapistID"].ToString();
                serviceMadeModel.Discounted = reader["discountID"].ToString();
            }
            conDB.closeConnection();
            return serviceMadeModel;
        }

        //GET PROMO SERVICES DETAILS
        private List<PromoServicesModel> getPromoServicesRendered(string id)
        {
            List<PromoServicesModel> lstPromoServices = new List<PromoServicesModel>();
            PromoServicesModel promoServ = new PromoServicesModel();

            string queryString = "SELECT dbspa.tblpromoservicemade.ID, therapistID, dbspa.tbltherapist.description, serviceID, " +
                "dbspa.tblservicetype.serviceType, promoservicesclientID FROM ((dbspa.tblpromoservicemade INNER JOIN dbspa.tbltherapist " +
                "ON dbspa.tblpromoservicemade.therapistID = dbspa.tbltherapist.ID INNER JOIN dbspa.tblservicetype " +
                "ON dbspa.tblpromoservicemade.serviceID = dbspa.tblservicetype.ID)) WHERE dbspa.tblpromoservicemade.promoservicesclientID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(id);
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while(reader.Read())
            {
                promoServ.ID = reader["ID"].ToString();
                promoServ.Therapist = reader["description"].ToString();
                promoServ.TherapistID = reader["therapistID"].ToString();
                promoServ.ServiceName = reader["serviceType"].ToString();
                promoServ.ServiceID = reader["serviceID"].ToString();
                promoServ.PromoID = reader["promoservicesclientID"].ToString();
                lstPromoServices.Add(promoServ);
                promoServ = new PromoServicesModel();

            }

            conDB.closeConnection();
            return lstPromoServices;
        }

        private bool checkFields()
        {
            bool ifCorrect = false;
            if(chkAvailPromo.IsChecked == true)
            {
                PromoModel p = cmbPromoServices.SelectedItem as PromoModel;
                if (p == null)
                {
                    MessageBox.Show("Please select Promos!");
                }else
                {
                    ifCorrect = true;
                }
            }
            else
            {
                if (cmbServices.SelectedItem == null)
                {
                    MessageBox.Show("Please select Service Type value!");
                }
                else if (cmbTherapist.SelectedItem == null)
                {
                    MessageBox.Show("Please select Therapist!");
                }
                else if (cmbDiscount.SelectedItem == null)
                {
                    MessageBox.Show("Please select Discount!");
                }
                else if (chkAvailPromo.IsChecked == true)
                {
                    PromoModel p = cmbPromoServices.SelectedItem as PromoModel;
                    if(p == null)
                    {
                        MessageBox.Show("Please select Promos!");
                    }
                }             
                else
                {
                    ifCorrect = true;
                }
            }
            

            return ifCorrect;

        }

        private CommissionModel getCommisionForService(int id)
        {
            CommissionModel cm = new CommissionModel();
            try
            {
                string queryString = "SELECT ID,serviceTypeID, commission FROM dbspa.tblcommissions WHERE (isDeleted = 0) AND (serviceTypeID = ?)";
                List<string> parameters = new List<string>();

                parameters.Add(id.ToString());

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    cm.ID1 = reader["ID"].ToString();
                    cm.ServiceTypeID = reader["serviceTypeID"].ToString();
                    cm.Commission = reader["commission"].ToString();
                }

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return cm;
        }

        
        private List<ServiceMadeModel> getServiceForClient(int id)
        {
            List<ServiceMadeModel> lstServiceMade = new List<ServiceMadeModel>();
            try
            {
                string queryString = "SELECT dbspa.tblservicemade.ID, dbspa.tblservicetype.servicetype AS 'SERVICE TYPE', " +
                    "dbspa.tbltherapist.description AS 'THERAPIST', dbspa.tblservicemade.dateServiced, dbspa.tblservicemade.discountID," +
                    "IF(dbspa.tblservicemade.isDiscounted, ROUND(dbspa.tblservicetype.price - dbspa.tblservicetype.price * (dbspa.tbldiscount.discount / 100)), dbspa.tblservicetype.price) AS 'PRICE', " +
                    "IF(dbspa.tblservicemade.isDiscounted, 'YES', 'NO') AS 'DISCOUNTED', IF(dbspa.tblservicemade.savetocard, 'YES', 'NO') AS 'SAVED TO CARD', " +
                    "IF(dbspa.tblservicemade.firstfree, 'YES', 'NO') AS 'FREE 30 MINS', IF(dbspa.tblservicemade.secondfree, 'YES', 'NO') AS 'FREE 1HR' " +
                    "FROM ((((dbspa.tblservicemade INNER JOIN dbspa.tblclient ON dbspa.tblservicemade.clientID = dbspa.tblClient.ID) " +
                    "INNER JOIN dbspa.tblservicetype ON tblservicemade.serviceTypeID = dbspa.tblservicetype.ID) " +
                    "INNER JOIN dbspa.tbltherapist ON dbspa.tblservicemade.therapistID = dbspa.tbltherapist.ID) " +
                    "INNER JOIN dbspa.tbldiscount ON dbspa.tblservicemade.discountID = dbspa.tbldiscount.ID) WHERE (dbspa.tblclient.ID = ?) " +
                    "AND (dbspa.tblservicemade.isDeleted = 0) ORDER BY ID desc";

                List<string> parameters = new List<string>();
                parameters.Add(id.ToString());

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
                while (reader.Read())
                {
                    ServiceMadeModel serviceMade = new ServiceMadeModel();
                    serviceMade.ID = reader["ID"].ToString();
                    DateTime dte = DateTime.Parse(reader["dateServiced"].ToString());
                    serviceMade.DateServiced = dte.ToShortDateString();
                    serviceMade.ServiceType = reader["SERVICE TYPE"].ToString();
                    serviceMade.Therapist = reader["THERAPIST"].ToString();
                    serviceMade.Price = reader["PRICE"].ToString();
                    serviceMade.Discounted = reader["DISCOUNTED"].ToString();
                    serviceMade.Discount = reader["discountID"].ToString();
                    serviceMade.isSavedToCard = reader["SAVED TO CARD"].ToString();
                    serviceMade.FirstFree = reader["FREE 30 MINS"].ToString();
                    serviceMade.SecondFree = reader["FREE 1HR"].ToString();
                    serviceMade.ifPromoService = false;
                    lstServiceMade.Add(serviceMade);
                }

                conDB.closeConnection();
                lstServiceMade.AddRange(getPromoServicesMadeForClient());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            
            return lstServiceMade;
        }


        #region SERIAL NUMBER AND COD

        private SerialNumberModel checkSerialNumber()
        {
            SerialNumberModel snm = new SerialNumberModel();
            try
            {
                string queryString = "SELECT ID, serialNumber FROM dbspa.tblserialnumber WHERE (ID =" +
                             "(SELECT MAX(ID) AS Expr1 FROM dbspa.tblserialnumber tblSerialNumber_1))";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    snm.ID1 = reader["ID"].ToString();
                    snm.SerialNumber = reader["serialNumber"].ToString();
                }

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return snm;
        }

        private CODModel checkCODNumber()
        {
            CODModel codMod = new CODModel();

            try
            {
                //-1 BEfore , 1 AFTER
                
                DateTime parameterDate = DateTime.Parse(clientModel.DateServiced);
                DateTime dateChecker = DateTime.Now;

                //string cutOffTime = Properties.Settings.Default.cutOffTime;
                //DateTime ct = DateTime.Parse(parameterDate.ToShortDateString() + " " + cutOffTime);

                //var diff = DateTime.Compare(parameterDate, dateChecker);

                //if (diff == 0)
                //{
                //    ct = DateTime.Parse(parameterDate.AddDays(1).ToShortDateString() + " " + cutOffTime);
                //    int a = DateTime.Compare(parameterDate, ct);
                //    if (a <= 0)
                //    {
                //        parameterDate = DateTime.Now;
                //    }
                //}
                //else
                //{
                //    parameterDate = parameterDate.AddDays(-1);
                //}

                string queryString = "SELECT ID, COD, dateServiced FROM dbspa.tblcod WHERE (dateServiced = ?)";

                List<string> parameters = new List<string>();

                //DateTime date = DateTime.Parse(clientModel.DateServiced);

                parameters.Add(parameterDate.Year + "/" + parameterDate.Month + "/" + parameterDate.Day);

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    codMod.ID1 = reader["ID"].ToString();
                    codMod.COD1 = reader["COD"].ToString();
                    codMod.DateServiced = Convert.ToDateTime(reader["dateServiced"].ToString());
                }
                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return codMod;
        }
        private bool insertSerialNumber(int serial)
        {
            bool ifInserted = false;
            try
            {
                string queryString = "INSERT INTO dbspa.tblserialnumber (serialNumber, dateServiced) VALUES(?, ?)";
                List<string> parameters = new List<string>();

                parameters.Add(serial.ToString());
                DateTime date = DateTime.Parse(clientModel.DateServiced);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                ifInserted = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return ifInserted;
        }

        private bool insertCODNumber(int cod)
        {
            bool ifInserted = false;
            try
            {
                string stringQuery = "INSERT INTO dbspa.tblcod (COD, dateServiced) VALUES (?,?)";
                List<string> parameters = new List<string>();

                parameters.Add(cod.ToString());
                DateTime date = DateTime.Parse(clientModel.DateServiced);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                conDB.AddRecordToDatabase(stringQuery, parameters);
                conDB.closeConnection();

                ifInserted = true;

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return ifInserted;
        }

        private bool updateCODNumber(int cod, int id)
        {
            bool ifUpdated = false;

            try
            {
                string queryString = "UPDATE dbspa.tblcod SET COD = ? WHERE ID = ?";
                List<string> parameters = new List<string>();

                parameters.Add(cod.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return ifUpdated;
        }

        #endregion


        #region INSERT AND UPDATE CLIENT RECORDS


        private int insertClientRecord(ClientModel model)
        {
            int x = 0;
            try
            {
                string insert = "INSERT INTO dbspa.tblclient (dateServiced,serialNo,cod,firstName,lastName,address,phonenumber,servicemode,totalamt,timeIn,timeOut,isDeleted,isLoyal,LoyaltyID) " +
                    "VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                List<string> parameters = new List<string>();

                DateTime date = DateTime.Parse(model.DateServiced);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
                parameters.Add(model.SerialNumber);
                parameters.Add(model.Cod);
                parameters.Add(model.FirstName);
                parameters.Add(model.LastName);
                parameters.Add(model.Address);
                parameters.Add(model.PhoneNumber);
                parameters.Add(model.ServiceMode);
                parameters.Add(" ");
                parameters.Add(model.TimeIn);
                parameters.Add(" ");
                parameters.Add(0.ToString());
                parameters.Add(model.isLoyal);
                parameters.Add(model.LoyaltyID);
                conDB.AddRecordToDatabase(insert, parameters);

                //GET ID OF INSERTED RECORD

                MySqlDataReader reader = conDB.getSelectConnection("select ID from dbspa.tblclient order by ID desc limit 1", null);

                while (reader.Read())
                {
                    x = Convert.ToInt32(reader["ID"].ToString());
                }

                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return x;
        }

        private void updateClientRecord(string serial, string cod, string sloyal, string sloyaltyid)
        {
            string queryString = "UPDATE dbspa.tblclient SET serial = ?, dateServiced = ? , cod = ? , isLoyal = ?, LoyaltyID = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(serial);
            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(cod);
            parameters.Add(sloyal);
            parameters.Add(sloyaltyid);
            parameters.Add(clientModel.ID1);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void updateClientRecordRemoveLoyaltyCard()
        {
            string queryString = "UPDATE dbspa.tblclient SET isLoyal = 0, LoyaltyID = 0 WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(clientModel.ID1);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        #endregion


        #region SERVICES AND LOYALTY CARDS
        private void insertServicesMade(ComboBox cmbService, ClientModel model, CommissionModel commission)
        {

            try
            {
                //GET ID OF THERAPIST
                TherapistModel tm1 = cmbTherapist.SelectedItem as TherapistModel;
                ServiceTypeModel stm = cmbService.SelectedItem as ServiceTypeModel;
                DiscountModel dm = cmbDiscount.SelectedItem as DiscountModel;

                //bool ifDiscounted = false;
                int numDiscounted = 0;
                if (Convert.ToInt32(dm.Discount) > 0)
                {
                    //ifDiscounted = true;
                    numDiscounted = 1;
                }

                string queryString = "INSERT INTO dbspa.tblservicemade (serviceTypeID,clientID,therapistID,dateServiced,isDeleted,isDiscounted, " +
                    "discountID,commissionID,loyaltyID,savetocard, firstfree, secondfree)" +
                    "VALUES(?,?, ?,?,?,?,?,?,?,?,?,?)";

                List<string> parameters = new List<string>();

                parameters.Add(Convert.ToInt32(stm.ID1).ToString());
                parameters.Add(Convert.ToInt32(model.ID1).ToString());
                parameters.Add(Convert.ToInt32(tm1.ID1).ToString());
                DateTime date = DateTime.Parse(model.DateServiced);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
                parameters.Add(0.ToString());
                parameters.Add(numDiscounted.ToString());
                parameters.Add(dm.ID1);
                parameters.Add(Convert.ToInt32(commission.ID1).ToString());

                parameters.Add(model.LoyaltyID);

                if (chkAvailToCard.IsChecked == true)
                {
                    parameters.Add("1");
                }
                else
                {
                    parameters.Add("0");
                }

                //AVAIL 30 MINS FREE
                if (chkFirstFree.IsChecked == true)
                {
                    parameters.Add("1");
                }
                else
                {
                    parameters.Add("0");
                }

                //AVAIL 1 HR free
                if (chkSecondFree.IsChecked == true)
                {
                    parameters.Add("1");
                }
                else
                {
                    parameters.Add("0");
                }

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();
                conDB.writeLogFile("ADDED SERVICE FOR CLIENT: " + stm.Description + " " + model.FirstName + " " + model.LastName);
                //updateLoyaltyRecord(model.ID1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateServiceRendered(ComboBox cmbService)
        {
            try
            {
                TherapistModel tm1 = cmbTherapist.SelectedItem as TherapistModel;
                ServiceTypeModel stm = cmbService.SelectedItem as ServiceTypeModel;
                DiscountModel dm = cmbDiscount.SelectedItem as DiscountModel;
                CommissionModel commissionModel = new CommissionModel();
                commissionModel = getCommisionForService(Convert.ToInt32(stm.ID1));

                //bool ifDiscounted = false;
                int numDiscounted = 0;
                if (Convert.ToInt32(dm.Discount) > 0)
                {
                    //ifDiscounted = true;
                    numDiscounted = 1;
                }


                string queryString = "UPDATE dbspa.tblservicemade SET serviceTypeID = ?, dateServiced = ?, isDiscounted = ?, discountID = ? " +
                    " ,therapistID = ? , commissionID = ?, savetocard = ? WHERE ID = ?";

                List<string> parameters = new List<string>();
                parameters.Add(stm.ID1);
                DateTime dte = DateTime.Parse(serviceMade.DateServiced);      
                parameters.Add(dte.Year + "/" + dte.Month + "/" + dte.Day);
                parameters.Add(numDiscounted.ToString());
                parameters.Add(dm.ID1);
                parameters.Add(tm1.ID1);
                parameters.Add(commissionModel.ID1);

                if (chkAvailToCard.IsChecked == true)
                {
                    parameters.Add("1");
                    updateLoyaltyRecordWithStamp(clientModel.ID1);
                }else
                {
                    parameters.Add("0");
                    if (serviceMade.isSavedToCard.Equals("YES") || serviceMade.isSavedToCard.Equals("1"))
                    {
                        updateLoyaltyRecordDeductStamp();
                    }
                }

                parameters.Add(serviceMade.ID);

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();
                conDB.writeLogFile("UPDATE SERVICE FOR CLIENT: " + stm.Description + " " + clientModel.FirstName + " " + clientModel.LastName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateServicesUnderPromoName()
        {

            PromoModel ps = cmbPromoServices.SelectedItem as PromoModel;


            for (int i = 0; i < (dgvPromoServices.Items.Count - 1); i++)
            {
                PromoServicesModel p = dgvPromoServices.Items[i] as PromoServicesModel;
                DataGridRow row = dgvPromoServices.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                ComboBox ele = dgvPromoServices.Columns[1].GetCellContent(row) as ComboBox;
                TherapistModel th = ele.SelectedItem as TherapistModel;

                string queryString = "UPDATE dbspa.tblpromoservicemade SET therapistID = ? WHERE ID = ?";

                List<string> parameters = new List<string>();
                if (th != null)
                {
                    parameters.Add(th.ID1);
                }
                else
                {
                    parameters.Add(p.TherapistID);
                }

                parameters.Add(p.ID);

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();
                conDB.writeLogFile("UPDATE PROMO SERVICE FOR CLIENT: THERAPIST: " + th.Description + " RECORD ID: " + p.ID);
            }
        }

        private void updateLoyaltyRecordWithStamp(string EmpId)
        {
            string queryString = "UPDATE dbspa.tblloyaltycard SET clientID = ?, servicecount = servicecount+1 WHERE ID = ?";
            List<string> parameters = new List<string>();

            parameters.Add(EmpId);
            parameters.Add(clientModel.LoyaltyID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("UPDATE LOYALTY CARD WITH STAMP: LOYALTY CARD ID : " + clientModel.LoyaltyID);

        }

        private void updateLoyaltyRecordWithoutStamp(string EmpId)
        {
            string queryString = "UPDATE dbspa.tblloyaltycard SET clientID = ? WHERE ID = ?";
            List<string> parameters = new List<string>();

            parameters.Add(EmpId);
            parameters.Add(clientModel.LoyaltyID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("UPDATE LOYALTY CARD WITHOUT STAMP: LOYALTY CARD ID : " + clientModel.LoyaltyID);
        }

        private void updateLoyaltyRecordDeductStamp()
        {
            string queryString = "UPDATE dbspa.tblloyaltycard SET servicecount = servicecount-1 WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(clientModel.LoyaltyID);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("UPDATE LOYALTY CARD DEDECUT STAMP: LOYALTY CARD ID : " + clientModel.LoyaltyID);
        }

        private int insertPromoServicesForClient(PromoModel p)
        {
            int x = 0;
            string queryString = "INSERT INTO dbspa.tblpromoservicesclient (clientID, promoID, promoprice, savetocard, loyaltyID, dateserviced, isDeleted) " +
                "VALUES (?,?,?,?,?,?,?)";
            List<string> parameters = new List<string>();

            parameters.Add(clientModel.ID1);
            parameters.Add(p.ID);
            parameters.Add(p.PromoPrice.ToString());

            if (chkAvailToCard.IsChecked == true)
            {
                parameters.Add("1");
            }
            else
            {
                parameters.Add("0");
            }

            parameters.Add(clientModel.LoyaltyID);
            DateTime date = DateTime.Parse(clientModel.DateServiced);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add("0");

            conDB.AddRecordToDatabase(queryString, parameters);

            MySqlDataReader reader = conDB.getSelectConnection("select ID from dbspa.tblpromoservicesclient order by ID desc limit 1", null);
            while (reader.Read())
            {
                x = Convert.ToInt32(reader["ID"].ToString());
            }

            conDB.closeConnection();
            conDB.writeLogFile("INSERT PROMO SERVICE FOR CLIENT: CLIENT: " + clientModel.ID1 + " RECORD ID: " + x);
            return x;
        }

        private void insertPromoServicesMade()
        {
            PromoModel ps = cmbPromoServices.SelectedItem as PromoModel;
            if (checkTherapistInDropdown())
            {
                int x = insertPromoServicesForClient(ps);

                for (int i = 0; i < (dgvPromoServices.Items.Count - 1); i++)
                {
                    PromoServicesModel p = dgvPromoServices.Items[i] as PromoServicesModel;
                    DataGridRow row = dgvPromoServices.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                    ComboBox ele = dgvPromoServices.Columns[1].GetCellContent(row) as ComboBox;
                    TherapistModel th = ele.SelectedItem as TherapistModel;

                    string queryString = "INSERT INTO dbspa.tblpromoservicemade (promoID, clientID, therapistID, serviceID, dateserviced, promoservicesclientID, isDeleted) " +
                        "VALUES (?,?,?,?,?,?,?)";

                    List<string> parameters = new List<string>();

                    parameters.Add(ps.ID);
                    parameters.Add(clientModel.ID1);
                    parameters.Add(th.ID1);
                    parameters.Add(p.ServiceID);

                    DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
                    parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                    parameters.Add(x.ToString());
                    parameters.Add("0");

                    conDB.AddRecordToDatabase(queryString, parameters);
                    conDB.closeConnection();
                    conDB.writeLogFile("INSERT PROMO SERVICE MADE FOR CLIENT: CLIENT: " + clientModel.ID1 + " RECORD ID: " + x);
                }

            }
        }

        private void updateLoyaltyCardFirstFree()
        {
            string queryString = "UPDATE dbspa.tblloyaltycard SET firstfree = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add("1");
            parameters.Add(clientModel.LoyaltyID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("UPDATE LOYALTY CARD: AVAIL FIRST FREE: LOYALTY CARD ID: " + clientModel.LoyaltyID);
        }

        private void updateLoyaltyCardSecondFree()
        {
            string queryString = "UPDATE dbspa.tblloyaltycard SET secondfree = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add("1");
            parameters.Add(clientModel.LoyaltyID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("UPDATE LOYALTY CARD: AVAIL SECOND FREE: LOYALTY CARD ID: " + clientModel.LoyaltyID);
        }

        private void updateLoyaltyCardRemoveLoyaltyCard()
        {
            string queryString = "UPDATE dbspa.tblloyaltycard SET isDeleted = 1 WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(clientModel.LoyaltyID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        #endregion


        #region UPDATE DATAGRID 
        private void updateClientView()
        {
            try
            {
                List<ClientModel> lstClient = new List<ClientModel>();

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
                    ClientModel client = new ClientModel();
                    client.ID1 = reader["ID"].ToString();
                    client.FirstName = reader["FIRST NAME"].ToString();
                    client.LastName = reader["LAST NAME"].ToString();
                    client.PhoneNumber = reader["phonenumber"].ToString();
                    client.ServiceMode = reader["SERVICE MODE"].ToString();
                    client.isLoyal = reader["LOYALTY CARD"].ToString();
                    client.LoyaltyID = reader["LoyaltyID"].ToString();
                    lstClient.Add(client);
                }
                cf.dgvClient.ItemsSource = lstClient;
                
                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }


        }

        private void loadServicesByPromoName()
        {
            PromoModel promoMod = cmbPromoServices.SelectedItem as PromoModel;
            PromoServicesModel promoServ = new PromoServicesModel();
            List<PromoServicesModel> lstPromoServ = new List<PromoServicesModel>();

            string queryString = "SELECT dbspa.tblpromoservices.promoID, dbspa.tblpromo.promoname, dbspa.tblpromo.price, dbspa.tblservicetype.serviceType, dbspa.tblservicetype.ID AS 'SERVICETYPEID' " +
                "FROM((dbspa.tblpromoservices INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservices.promoID = dbspa.tblpromo.ID) INNER JOIN " +
                "dbspa.tblservicetype ON dbspa.tblpromoservices.serviceID = dbspa.tblservicetype.ID) WHERE dbspa.tblpromoservices.isDeleted = 0 " +
                "AND dbspa.tblpromoservices.promoID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(promoMod.ID);
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                promoServ.ID = reader["promoID"].ToString();
                promoServ.PromoName = reader["promoname"].ToString();
                promoServ.PromoPrice = reader["price"].ToString();
                promoServ.ServiceName = reader["serviceType"].ToString();
                promoServ.ServiceID = reader["SERVICETYPEID"].ToString();
                lstPromoServ.Add(promoServ);
                promoServ = new PromoServicesModel();
            }
            conDB.closeConnection();
            dgvPromoServices.ItemsSource = lstPromoServ;

        }

        private List<ServiceMadeModel> getPromoServicesMadeForClient()
        {
            List<ServiceMadeModel> lstPromoServices = new List<ServiceMadeModel>();
            ServiceMadeModel s = new ServiceMadeModel();

            string queryString = "SELECT dbspa.tblpromoservicesclient.ID, clientID, promoID, promoprice, loyaltyID, dbspa.tblpromo.promoname,dbspa.tblpromoservicesclient.dateserviced, " +
                "IF(dbspa.tblpromoservicesclient.savetocard, 'YES', 'NO') AS 'SAVED TO CARD' FROM (dbspa.tblpromoservicesclient INNER JOIN dbspa.tblpromo " +
                "ON dbspa.tblpromoservicesclient.promoID = dbspa.tblpromo.ID) WHERE clientID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(clientModel.ID1);
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                s.ID = reader["ID"].ToString();
                s.PromoServicesClientID = reader["clientID"].ToString();
                s.ServiceType = reader["promoname"].ToString();
                s.PromoID = reader["promoID"].ToString();
                s.DateServiced = reader["dateserviced"].ToString();
                s.Price = reader["promoprice"].ToString();
                s.isSavedToCard = reader["SAVED TO CARD"].ToString();
                s.ifPromoService = true;
                lstPromoServices.Add(s);
                s = new ServiceMadeModel();
            }

            conDB.closeConnection();

            return lstPromoServices;
        }

        #endregion        

        private void disableRegularServiceFields()
        {
            cmbServices.IsEnabled = false;
            cmbTherapist.IsEnabled = false;
            cmbDiscount.IsEnabled = false;
            chkAvailToCard.IsEnabled = false;
            chkFirstFree.IsEnabled = false;
            chkSecondFree.IsEnabled = false;
        }

        private void enableRegularServiceFields()
        {
            cmbServices.IsEnabled = true;
            cmbTherapist.IsEnabled = true;
            cmbDiscount.IsEnabled = true;
            chkAvailToCard.IsEnabled = true;
            chkFirstFree.IsEnabled = true;
            chkSecondFree.IsEnabled = true;
        }

        private bool checkTherapistInDropdown()
        {
            bool ifAllCorrect = false;

            for (int i = 0; i < (dgvPromoServices.Items.Count - 1); i++)
            {
                DataGridRow row = dgvPromoServices.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                ComboBox ele = dgvPromoServices.Columns[1].GetCellContent(row) as ComboBox;
                TherapistModel th = ele.SelectedItem as TherapistModel;

                if (th == null)
                {
                    MessageBox.Show("Please select Therapist!");
                    ifAllCorrect = false;
                    break;
                }

                ifAllCorrect = true;
            }
                

            

            return ifAllCorrect;
        }

        
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (serviceMade.ifPromoService)
            {
                updateServicesUnderPromoName();
                MessageBox.Show("SERVICE UPDATED SUCCESSFULLY!");
                conDB.writeLogFile("SERVICE UPDATED: FOR CLIENT RECORD: " + clientModel.ID1);
                clientDet.dgvServiceMade.ItemsSource = getServiceForClient(Convert.ToInt32(clientModel.ID1));
                this.Close();
            }
            else
            {
                updateServiceRendered(cmbServices);
                MessageBox.Show("SERVICE UPDATED SUCCESSFULLY!");
                conDB.writeLogFile("SERVICE UPDATED: FOR CLIENT RECORD: " + clientModel.ID1);
                clientDet.dgvServiceMade.ItemsSource = getServiceForClient(Convert.ToInt32(clientModel.ID1));
                this.Close();
            }
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {               
                if (checkFields())
                {
                    CommissionModel commissionModel = new CommissionModel();
                    if (chkAvailPromo.IsChecked == false)
                    {
                        
                        ServiceTypeModel stm = cmbServices.SelectedItem as ServiceTypeModel;
                        commissionModel = getCommisionForService(Convert.ToInt32(stm.ID1));
                    }
                    

                    if (clientModel.ID1 != "" && clientModel.ID1 != null)
                    {
                        // get user type if admin or receptionist
                        int serial = 0;


                        CODModel codMod = checkCODNumber();

                        int codID = Convert.ToInt32(codMod.COD1);
                        if (codID == 0)
                        {
                            codID++;
                            clientModel.Cod = codID.ToString();
                            insertCODNumber(Convert.ToInt32(clientModel.Cod));
                            //clientModel.Cod = (codID + 1).ToString();
                            codID = Convert.ToInt32(clientModel.Cod);
                        }
                        else
                        {
                            codID++;
                            updateCODNumber(Convert.ToInt32(codID), Convert.ToInt32(codMod.ID1));
                            //clientModel.Cod = (codID + 1).ToString();
                            //
                        }

                        SerialNumberModel serialMod = checkSerialNumber();

                        serial = Convert.ToInt32(serialMod.SerialNumber);
                        insertSerialNumber((serial + 1));

                        if (user != null)
                        {
                            if(user.Type > 1)
                            {
                                
                            }
                        }
                       
                        //IF CLIENT AVAIL PROMO 
                        if (chkAvailPromo.IsChecked == true)
                        {
                            insertPromoServicesMade();
                        }
                        else
                        {
                            insertServicesMade(cmbServices, clientModel, commissionModel);
                        }

                        //if STAMP TO CARD
                        if (chkAvailToCard.IsChecked == true && (chkFirstFree.IsChecked == false && chkSecondFree.IsChecked == false ))
                        {
                            updateLoyaltyRecordWithStamp(clientModel.ID1);
                        }else if(Convert.ToInt32(clientModel.LoyaltyID) > 1)
                        {
                            updateLoyaltyRecordWithoutStamp(clientModel.ID1);
                        }

                        //MARK FIRST FREE
                        if(chkFirstFree.IsChecked == true)
                        {
                            updateLoyaltyCardFirstFree();
                        }


                        //MARK SECOND FREE
                        bool boolSecondFree = false;
                        if(chkSecondFree.IsChecked == true)
                        {
                           
                            updateLoyaltyCardSecondFree();
                            boolSecondFree = true;
                        }

                        updateClientRecord((serial + 1).ToString(),codID.ToString(), clientModel.isLoyal, clientModel.LoyaltyID);

                        if (Convert.ToInt32(clientModel.ServiceCount) >= 10 && (clientModel.FirstFree.Equals("1")) && 
                            (chkSecondFree.IsChecked == true || boolSecondFree))
                        {
                            updateClientRecordRemoveLoyaltyCard();
                            updateLoyaltyCardRemoveLoyaltyCard();
                        }
                        clientDet.txtSerialNumber.Text = (serial + 1).ToString();
                        clientDet.txtCOD.Text = codID.ToString();
                        clientDet.txtDate.Text = clientModel.DateServiced;
                        clientDet.dgvServiceMade.ItemsSource = getServiceForClient(Convert.ToInt32(clientModel.ID1));
                        MessageBox.Show("SERVICE ADDED SUCCESSFULLY!");
                        
                        this.Close();
                    }
                    else
                    {
                        // get user type if admin or receptionist
                        SerialNumberModel serialMod = checkSerialNumber();
                        int serial = Convert.ToInt32(serialMod.SerialNumber);
                        CODModel codMod = checkCODNumber();
                        int codID = Convert.ToInt32(codMod.COD1);

                        if (user != null)
                        {
                            if(user.Type > 1)
                            {

                                insertSerialNumber((serial + 1));
                                if (codID == 0)
                                {
                                    insertCODNumber((codID + 1));
                                    clientModel.Cod = (codID + 1).ToString();
                                    clientModel.SerialNumber = (serial + 1).ToString();
                                }
                                else
                                {
                                    updateCODNumber((codID + 1), Convert.ToInt32(codMod.ID1));
                                    clientModel.Cod = (codID + 1).ToString();
                                    clientModel.SerialNumber = (serial + 1).ToString();
                                }
                            }else
                            {
                            // if user is admin, still insert serial for continuity purpose on user end
                                if(Convert.ToInt32(clientModel.SerialNumber) > serial)
                                {
                                    insertSerialNumber(Convert.ToInt32(clientModel.SerialNumber));
                                }

                                if (codID == 0)
                                {
                                    insertCODNumber(Convert.ToInt32(clientModel.Cod));
                                    //updateCODNumber(Convert.ToInt32(clientModel.Cod), Convert.ToInt32(codMod.ID1));
                                }
                                else
                                {
                                    if (Convert.ToInt32(clientModel.Cod) > codID)
                                    {
                                        updateCODNumber(Convert.ToInt32(clientModel.Cod), Convert.ToInt32(codMod.ID1));
                                    }
                                } 
                            }
                        }

                        

                        int x = insertClientRecord(clientModel);

                        if (chkAvailToCard.IsChecked == true)
                        {
                            updateLoyaltyRecordWithStamp(x.ToString());
                        }
                        else if (Convert.ToInt32(clientModel.LoyaltyID) > 1)
                        {
                            updateLoyaltyRecordWithoutStamp(x.ToString());
                        }

                        clientModel.ID1 = x.ToString();

                        //IF CLIENT AVAIL PROMO 
                        if(chkAvailPromo.IsChecked == true)
                        {
                            insertPromoServicesMade();
                        }else
                        {
                            insertServicesMade(cmbServices, clientModel, commissionModel);
                        }

                        if (chkFirstFree.IsChecked == true)
                        {
                            updateLoyaltyCardFirstFree();
                        }


                        clientDet.ifAlreadySaved = true;
                        clientDet.clientModel = clientModel;
                        clientDet.dgvServiceMade.ItemsSource = getServiceForClient(x).OrderByDescending(srv => srv.DateServiced).ToList(); ;
                        updateClientView();
                        
                        MessageBox.Show("SERVICE ADDED SUCCESSFULLY!");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chkFirstFree_Checked(object sender, RoutedEventArgs e)
        {
            chkAvailToCard.IsChecked = true;
        }

        private void chkFirstFree_Unchecked(object sender, RoutedEventArgs e)
        {
            chkAvailToCard.IsEnabled = true;
        }

        private void chkSecondFree_Checked(object sender, RoutedEventArgs e)
        {
            chkAvailToCard.IsChecked = true;
        }

        private void chkSecondFree_Unchecked(object sender, RoutedEventArgs e)
        {
            chkAvailToCard.IsEnabled = true;
        }

        private void cmbPromoServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(serviceMade == null)
            {
                loadServicesByPromoName();
            }
            
        }

        private void chkAvailPromo_Checked(object sender, RoutedEventArgs e)
        {
            disableRegularServiceFields();
            cmbPromoServices.IsEnabled = true;
        }

        private void chkAvailPromo_Unchecked(object sender, RoutedEventArgs e)
        {
            enableRegularServiceFields();
            cmbPromoServices.IsEnabled = false;
        }

        private void chkAvailToCard_Unchecked(object sender, RoutedEventArgs e)
        {
            chkFirstFree.IsChecked = false;
            chkSecondFree.IsChecked = false;
        }
    }
}

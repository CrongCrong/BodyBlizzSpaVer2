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
using Xceed.Wpf.Toolkit;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ClientDetails.xaml
    /// </summary>
    public partial class ClientDetails : MetroWindow
    {
        public ClientDetails()
        {
            InitializeComponent();
        }

        ClientForm clientForm;
        public ClientModel clientModel;
        public bool ifAlreadySaved = false;
        ConnectionDB conDB = new ConnectionDB();
        User user;

        public ClientDetails(ClientForm fm)
        {
            clientForm = fm;
            InitializeComponent();
        }

        public ClientDetails(ClientForm fm, ClientModel cm, User usr)
        {
            clientForm = fm;
            clientModel = cm;
            user = usr;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cmbLoyaltycards.Visibility = Visibility.Hidden;

            txtDate.Visibility = Visibility.Hidden;
            
            timeInPicker.Text = DateTime.Now.ToShortTimeString();
            timeOutPicker.IsEnabled = false;
            if(user.Type > 1)
            {
                txtSerialNumber.IsEnabled = false;
                txtCOD.IsEnabled = false;
            }else
            {
                txtSerialNumber.IsEnabled = true;
                txtCOD.IsEnabled = true;
                SerialNumberModel serialNum = checkSerialNumber();
                txtSerialNumber.Text = (Convert.ToInt32(serialNum.SerialNumber) + 1).ToString();
                CODModel codNum = checkCODNumber(DateTime.Now.ToShortDateString());
                txtCOD.Text = (Convert.ToInt32(codNum.COD1) + 1).ToString();
            }
            

            if (clientModel != null)
            {
                fillComboBoxServiceMode(cmbServiceMode);
                fillLoyaltyCards();

                if (clientModel.IfViewDetails)
                {
                    datePicker.Visibility = Visibility.Hidden;
                    lblDate.Visibility = Visibility.Visible;
                    txtDate.Visibility = Visibility.Visible;
                    btnDeleteProduct.IsEnabled = false;
                    InitializeDetails();
                    cmbServiceMode.IsEnabled = false;
                    txtSerialNumber.IsEnabled = false;
                    txtCOD.IsEnabled = false;
                    timeInPicker.IsEnabled = false;
                    timeOutPicker.IsEnabled = false;
                    if (string.IsNullOrEmpty(clientModel.TimeOut) || string.IsNullOrWhiteSpace(clientModel.TimeOut))
                    {
                        timeOutPicker.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        timeOutPicker.Text = clientModel.TimeOut;
                    }

                    if (Convert.ToInt32(clientModel.LoyaltyID) > 0)
                    {
                        cmbLoyaltycards.Visibility = Visibility.Hidden;
                        chkAvailLoyalty.Visibility = Visibility.Hidden;
                    }

                    txtFirstName.IsEnabled = false;
                    txtLastName.IsEnabled = false;
                    txtAddress.IsEnabled = false;
                    txtPhoneNumber.IsEnabled = false;
                    txtDate.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                    btnAdd.IsEnabled = false;
                    btnEdit.IsEnabled = false;
                    btnAddProduct.IsEnabled = false;

                }
                else if (clientModel.IfEditDetails)
                {
                    datePicker.Visibility = Visibility.Visible;
                    lblDate.Visibility = Visibility.Visible;
                    txtDate.Visibility = Visibility.Hidden;
                    txtDate.Visibility = Visibility.Hidden;

                    InitializeDetails();

                    txtFirstName.IsEnabled = true;
                    txtLastName.IsEnabled = true;
                    txtAddress.IsEnabled = true;
                    txtPhoneNumber.IsEnabled = true;
                    timeOutPicker.IsEnabled = true;

                    if (Convert.ToInt32(clientModel.LoyaltyID) > 0)
                    {
                        cmbLoyaltycards.Visibility = Visibility.Hidden;
                        chkAvailLoyalty.Visibility = Visibility.Hidden;
                    }

                    if (string.IsNullOrEmpty(clientModel.TimeOut) || string.IsNullOrWhiteSpace(clientModel.TimeOut))
                    {
                        //Initialize Time Out picker
                        //timeOutPicker.Format = DateTimePickerFormat.Custom;
                        //timeOutPicker.CustomFormat = "hh:mm tt"; // Only use hours and minutes
                        //timeOutPicker.ShowUpDown = true;
                    }
                    else
                    {
                        timeOutPicker.Text = clientModel.TimeOut;
                    }
                    btnSave.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                    btnAdd.IsEnabled = true;
                }
            }
        }

        private void InitializeDetails()
        {
            //dateTimePicker1.Visible = false;
            txtDate.Visibility = Visibility.Visible;
            DateTime date = DateTime.Parse(clientModel.DateServiced);
            txtDate.Text = date.ToShortDateString();

            //txtSerialNumber.IsEnabled = false;
            //txtCOD.IsEnabled = false;

            //cmbServiceMode.IsEnabled = false;
            //timeInPicker.IsEnabled = false;
            //timeOutPicker.IsEnabled = false;

            //txtSerialNumber.Text = clientModel.SerialNumber.PadLeft(6, '0');
            //txtCOD.Text = clientModel.Cod.PadLeft(3, '0');
            txtSerialNumber.Text = clientModel.SerialNumber;
            txtCOD.Text = clientModel.Cod;
            txtFirstName.Text = clientModel.FirstName;
            txtLastName.Text = clientModel.LastName;
            txtAddress.Text = clientModel.Address;
            txtPhoneNumber.Text = clientModel.PhoneNumber;
            //txtTotal.Text = clientModel.TotalAmt;

            fillComboBoxServiceMode(cmbServiceMode);
            cmbServiceMode.SelectedItem = clientModel.ServiceMode;

            timeInPicker.Text = clientModel.TimeIn;


            //fill datagrid view for services
            dgvServiceMade.ItemsSource = getServiceForClient(Convert.ToInt32(clientModel.ID1));
            dgvClientProduct.ItemsSource = getProductsBoughtForClient(Convert.ToInt32(clientModel.ID1));

        }

        private void fillLoyaltyCards()
        {
            string queryString = "SELECT ID, serialnumber FROM dbspa.tblloyaltycard WHERE isDeleted = 0 AND clientID = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                LoyaltyCardModel lcm = new LoyaltyCardModel();
                lcm.ID = reader["ID"].ToString();
                lcm.SerialNumber = reader["serialnumber"].ToString();

                cmbLoyaltycards.Items.Add(lcm);

                if (clientModel != null)
                {
                    if(lcm.ID.Equals(clientModel.LoyaltyID))
                    {
                        cmbLoyaltycards.SelectedItem = lcm;
                    }
                }
            }
        }

        public void fillComboBoxServiceMode(ComboBox combo)
        {
            try
            {
                string queryString = "SELECT ID, serviceType FROM dbspa.tblservicemode WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    ServiceModeModel smm = new ServiceModeModel();
                    smm.ID1 = reader["ID"].ToString();
                    smm.ServiceType = reader["serviceType"].ToString();
                    combo.Items.Add(smm);
                    if (clientModel != null)
                    {
                        if(smm.ID1.Equals(clientModel.ServiceMode))
                        {
                            combo.SelectedItem = smm;
                        }
                    }
                }


                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

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
                lstServiceMade.AddRange(getPromoServicesMadeForClient());
                conDB.closeConnection();
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            return lstServiceMade;
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
                "WHERE dbspa.tblproductbought.isDeleted = 0 AND dbspa.tblproductbought.clientID = ? ORDER BY dbspa.tblproductbought.ID desc";

            List<string> parameters = new List<string>();
            parameters.Add(id.ToString());

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while(reader.Read())
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(datePicker.Text))
                {
                    if (!string.IsNullOrEmpty(clientModel.ID1))
                    {
                        if (clientModel.LoyaltyID.Equals("0"))
                        {
                            if (chkAvailLoyalty.IsChecked == true)
                            {
                                clientModel.LoyaltyID = cmbLoyaltycards.SelectedValue.ToString();
                                clientModel.isLoyal = "1";
                            }
                            else
                            {
                                clientModel.LoyaltyID = "0";
                                clientModel.isLoyal = "0";
                            }
                        }
                        DateTime dte = DateTime.Parse(datePicker.ToString());
                        clientModel.DateServiced = dte.Year + "/" + dte.Month + "/" + dte.Day;

                        ServiceForm serviceForm = new ServiceForm(this, clientForm, clientModel, user);
                        serviceForm.ShowDialog();
                    }
                    else
                    {
                        if (checkFields())
                        {
                            ClientModel model = new ClientModel();

                            model.DateServiced = datePicker.Text;
                            model.SerialNumber = txtSerialNumber.Text;
                            model.Cod = txtCOD.Text;
                            model.FirstName = txtFirstName.Text;
                            model.LastName = txtLastName.Text;
                            model.Address = txtAddress.Text;
                            model.PhoneNumber = txtPhoneNumber.Text;
                            model.TimeIn = timeInPicker.Text;
                            model.TimeOut = timeOutPicker.Text;
                            model.ServiceMode = cmbServiceMode.SelectedValue.ToString();

                            if (chkAvailLoyalty.IsChecked == true)
                            {
                                model.LoyaltyID = cmbLoyaltycards.SelectedValue.ToString();
                                model.isLoyal = "1";
                            }
                            else
                            {
                                model.LoyaltyID = "0";
                                model.isLoyal = "0";
                            }

                            ServiceForm serviceForm = new ServiceForm(this, clientForm, model, user);
                            serviceForm.ShowDialog();
                        }
                    }
                }else
                {
                    System.Windows.Forms.MessageBox.Show("Please select date.");
                }

                

                

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private bool checkFields()
        {
            bool ifContinue = false;

            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                System.Windows.MessageBox.Show("Please input First name of client.");
            }
            else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                System.Windows.MessageBox.Show("Please input Last name of client.");
            }
            else if (string.IsNullOrEmpty(txtAddress.Text))
            {
                System.Windows.MessageBox.Show("Please input Address of client.");
            }
            else if (string.IsNullOrEmpty(datePicker.Text))
            {
                System.Windows.MessageBox.Show("Please select Date.");
            }else if (user.Type < 1)
            {
                if (string.IsNullOrEmpty(txtSerialNumber.Text))
                {
                    System.Windows.MessageBox.Show("Please input Serial Number!");
                }
                
            }else if (user.Type < 1)
            {
                if (string.IsNullOrEmpty(txtCOD.Text))
                {
                    System.Windows.MessageBox.Show("Please input COD!");
                }
               
            }

            else if(cmbServiceMode.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select Service mode!");
            }else if(chkAvailLoyalty.IsChecked == true && cmbLoyaltycards.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select Loyalty Card!");
            }
            else
            {
                ifContinue = true;
            }

            return ifContinue;
        }

        private bool checkFieldsForUpdating()
        {
            bool ifContinue = false;

            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                System.Windows.MessageBox.Show("Please input First name of client.");
            }
            else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                System.Windows.MessageBox.Show("Please input Last name of client.");
            }
            else if (string.IsNullOrEmpty(txtAddress.Text))
            {
                System.Windows.MessageBox.Show("Please input Address of client.");
            }
            else if (string.IsNullOrEmpty(txtSerialNumber.Text))
            {
                System.Windows.MessageBox.Show("Please input Serial Number!");
            }
            else if (string.IsNullOrEmpty(txtCOD.Text))
            {
                System.Windows.MessageBox.Show("Please input COD!");
            }

            else if (cmbServiceMode.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select Service mode!");
            }
            else if (chkAvailLoyalty.IsChecked == true && cmbLoyaltycards.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select Loyalty Card!");
            }
            else if (string.IsNullOrEmpty(datePicker.Text))
            {
                System.Windows.MessageBox.Show("Please select date.");
            }
            else
            {
                ifContinue = true;
            }

            return ifContinue;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ServiceMadeModel servMade = dgvServiceMade.SelectedItem as ServiceMadeModel;
            if(servMade != null)
            {
                if (!string.IsNullOrEmpty(datePicker.Text))
                {
                    DateTime date1 = DateTime.Parse(servMade.DateServiced);
                    string strDate = DateTime.Now.ToShortDateString();
                    DateTime now = DateTime.Parse(strDate);
                    if (date1 < now && user.Type > 1)
                    {
                        System.Windows.MessageBox.Show("Sorry. You can't edit previous rendered service!");
                    }
                    else
                    {
                        servMade.DateServiced = datePicker.Text;
                        ServiceForm service = new ServiceForm(this, servMade, clientModel, user);
                        service.ShowDialog();
                    }
                }else
                {
                    System.Windows.MessageBox.Show("Please select date.");
                }
                
            }else
            {
                System.Windows.MessageBox.Show("No record selected!");
            }
            
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int serial = 0;

                if (!clientModel.IfEditDetails && !clientModel.IfViewDetails)
                {
                    if (!ifAlreadySaved)
                    {
                        if (checkFields())
                        {
                            //Check user logged if admin or receptionist
                            if(user != null)
                            {
                                SerialNumberModel serialNum = checkSerialNumber();
                                CODModel codMod = checkCODNumber(datePicker.Text);
                                int x = Convert.ToInt32(codMod.COD1);
                                serial = Convert.ToInt32(serialNum.SerialNumber);
                                int numSerial = 0;
                                int numCod = 0;
                                if(user.Type == 1)
                                {
                                    numSerial = Convert.ToInt32(txtSerialNumber.Text);
                                    numCod = Convert.ToInt32(txtCOD.Text);

                                    if (numSerial > serial)
                                    {
                                        serial = numSerial;
                                    }


                                }else
                                {
                                    serial += 1;
                                }
                                

                                //CHECK SERIAL NUMBER
                                insertSerialNumber((serial), datePicker.Text);

                                //CHECK COD
                                if (x == 0)
                                {

                                    insertCODNumber(numCod);
                                }
                                else
                                {
                                    if(user.Type == 1)
                                    {
                                        if (numCod > x)
                                        {
                                            updateCODNumber(numCod, Convert.ToInt32(codMod.ID1));
                                        }
                                    }else
                                    {
                                        numCod = x + 1;
                                        updateCODNumber(numCod, Convert.ToInt32(codMod.ID1));
                                    }

                                }

                                //serial = serial + 1;
                                //x = x + 1;
                                if (insertClientRecord(serial.ToString(), numCod.ToString() ))
                                {
                                    updateClientView();
                                    System.Windows.MessageBox.Show("Client Record Successfully Saved!");
                                    this.Close();
                                }
                            }
                            else
                            {
                                //original code insertClientRecord((serial + 1), (x + 1))
                             

                                if (insertClientRecord(txtSerialNumber.Text, txtCOD.Text))
                                {
                                    updateClientView();
                                    System.Windows.MessageBox.Show("Client Record Successfully Saved!");
                                    this.Close();
                                }
                            }
                            

                            
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Client already exist!");
                        this.Close();
                    }
                }
                else
                {
                    if (checkFieldsForUpdating())
                    {
                        if (user.Type == 1)
                        {
                            insertSerialNumber(Convert.ToInt32(txtSerialNumber.Text), txtDate.Text);
                        }else
                        {
                            SerialNumberModel serialNum = checkSerialNumber();
                            serial = Convert.ToInt32(serialNum.SerialNumber);
                            serial += 1;
                        }

                        if (updateClientRecord(Convert.ToInt32(clientModel.ID1)))
                        {
                            System.Windows.MessageBox.Show("Record updated successfully!");
                            conDB.writeLogFile("SAVE CLIENT RECORD: UPDATED RECORD ID: " + clientModel.ID1);
                            updateClientView();
                            this.Close();
                        }
                    }
                    

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

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

        private CODModel checkCODNumber(string dte)
        {
            CODModel codMod = new CODModel();

            try
            {
                string queryString = "SELECT ID, COD, dateServiced FROM dbspa.tblcod WHERE (dateServiced = ?)";

                List<string> parameters = new List<string>();

                DateTime date = DateTime.Parse(dte);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

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

        private bool insertSerialNumber(int serial, string textDate)
        {
            bool ifInserted = false;
            try
            {
                string queryString = "INSERT INTO dbspa.tblserialnumber (serialNumber, dateServiced) VALUES(?, ?)";
                List<string> parameters = new List<string>();

                parameters.Add(serial.ToString());
                DateTime date = DateTime.Parse(textDate);
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
                string stringQuery = "INSERT INTO dbspa.tblcod (COD, dateServiced)VALUES(?,?)";
                List<string> parameters = new List<string>();

                parameters.Add(cod.ToString());
                DateTime date = DateTime.Parse(datePicker.Text);
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
      
        private bool insertClientRecord(string serial, string cod)
        {
            bool allCorrect = false;
            LoyaltyCardModel loyalCard = cmbLoyaltycards.SelectedItem as LoyaltyCardModel;
            try
            {
                string queryString = "INSERT INTO dbspa.tblclient (dateServiced, datejoined,serialNo,cod,firstName,lastName,address,phonenumber,servicemode,totalamt,timeIn,timeOut,isDeleted,isLoyal,LoyaltyID) " +
                    " VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                List<string> parameters = new List<string>();

                DateTime date = DateTime.Parse(datePicker.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

                parameters.Add(serial.ToString());
                parameters.Add(cod.ToString());
                parameters.Add(txtFirstName.Text);
                parameters.Add(txtLastName.Text);
                parameters.Add(txtAddress.Text);
                if (string.IsNullOrEmpty(txtPhoneNumber.Text))
                {
                    parameters.Add("0");
                }else
                {
                    parameters.Add(txtPhoneNumber.Text);
                }
                parameters.Add(cmbServiceMode.SelectedValue.ToString());
                parameters.Add("0");
                parameters.Add(timeInPicker.Text);
                parameters.Add("0");
                parameters.Add(0.ToString());

                if (chkAvailLoyalty.IsChecked == true)
                {
                    parameters.Add("1");

                    if (loyalCard != null)
                    {
                        parameters.Add(loyalCard.ID);
                    }

                }
                else
                {
                    parameters.Add("0");
                    parameters.Add("0");
                }

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                allCorrect = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to save record!" + ex.Message);
                Console.WriteLine(ex.Message);
                allCorrect = false;
            }

            return allCorrect;

        }

        private void updateClientView()
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

            clientForm.dgvClient.ItemsSource = lstClient;

        }

        private bool updateClientRecord(int id)
        {
            bool correct = false;
            try
            {

                string queryString = "UPDATE dbspa.tblclient SET dateServiced = ?, servicemode = ?, serialNo = ?, cod = ?, " +
                    "firstName = ?, lastName = ?, address = ?, phonenumber = ?, timeIn = ?, timeOut = ? WHERE ID = ?";

                List<string> parameters = new List<string>();
                DateTime date = DateTime.Parse(datePicker.Text);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
                parameters.Add(cmbServiceMode.SelectedValue.ToString());
                parameters.Add(txtSerialNumber.Text);
                parameters.Add(txtCOD.Text);
                parameters.Add(txtFirstName.Text);
                parameters.Add(txtLastName.Text);
                parameters.Add(txtAddress.Text);
                if (string.IsNullOrEmpty(txtPhoneNumber.Text))
                {
                    parameters.Add("0");
                }
                else
                {
                    parameters.Add(txtPhoneNumber.Text);
                }
                if (string.IsNullOrEmpty(timeInPicker.Text))
                {
                    parameters.Add("");
                }
                else
                {
                    parameters.Add(timeInPicker.Text);
                }

                if (string.IsNullOrEmpty(timeOutPicker.Text))
                {
                    parameters.Add("");
                }else
                {
                    parameters.Add(timeOutPicker.Text);
                }
                
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();
                correct = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            return correct;
        }

        private void deleteServiceMadeRecord(int id)
        {

            try
            {
                string queryString = "UPDATE dbspa.tblservicemade SET isDeleted = ? WHERE ID = ?";
                List<string> parameters = new List<string>();

                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void deleteProductBought(string strId)
        {
            string queryString = "UPDATE dbspa.tblproductbought SET isDeleted = 1 WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(strId);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                ServiceMadeModel cm = dgvServiceMade.SelectedItem as ServiceMadeModel;

                if (cm != null)
                {
                    int id = Convert.ToInt32(cm.ID);

                    if (id != 0)
                    {
                        deleteServiceMadeRecord(id);
                        dgvServiceMade.ItemsSource = getServiceForClient(Convert.ToInt32(clientModel.ID1));
                        System.Windows.MessageBox.Show("Record deleted successfuly!");
                        conDB.writeLogFile("DELETE CLIENT RECORD: DELETED CLIENT ID: " + cm.ID);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No record selected!");
                }
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(clientModel.ID1))
            {
                ProductForm prodForm = new ProductForm(this, clientModel);
                prodForm.ShowDialog();
            }
            else
            {
                if (checkFields())
                {
                    ClientModel model = new ClientModel();

                    model.DateServiced = DateTime.Now.ToShortDateString();
                    model.SerialNumber = txtSerialNumber.Text;
                    model.Cod = txtCOD.Text;
                    model.FirstName = txtFirstName.Text;
                    model.LastName = txtLastName.Text;
                    model.Address = txtAddress.Text;
                    model.TimeIn = timeInPicker.Text;
                    model.TimeOut = timeOutPicker.Text;
                    model.ServiceMode = cmbServiceMode.SelectedValue.ToString();

                    ProductForm prodForm = new ProductForm(this, model);
                    prodForm.ShowDialog();
                }
            }
            
        }

        private void chkAvailLoyalty_Checked(object sender, RoutedEventArgs e)
        {
            cmbLoyaltycards.Visibility = Visibility.Visible;
        }

        private void chkAvailLoyalty_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbLoyaltycards.Visibility = Visibility.Hidden;
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                ProductBoughtModel pb = dgvClientProduct.SelectedItem as ProductBoughtModel;
                if(pb != null)
                {
                    deleteProductBought(pb.ID);
                    dgvClientProduct.ItemsSource = getProductsBoughtForClient(Convert.ToInt32(clientModel.ID1));
                    System.Windows.MessageBox.Show("RECORD DELETED SUCCESSFULLY!");
                    conDB.writeLogFile("DELETED PRODUCTS: RECORD ID : " + pb.ID);
                }
                
            }
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SerialNumberModel serialN = checkSerialNumber();
            string strD = datePicker.Text;
            
            if (string.IsNullOrEmpty(strD))
            {
                strD = DateTime.Now.ToShortDateString();
            }

            CODModel codnum = checkCODNumber(strD);
            txtCOD.Text = (Convert.ToInt32(codnum.COD1) + 1).ToString();
            txtSerialNumber.Text = (Convert.ToInt32(serialN.SerialNumber)  + 1).ToString();
        }
    }
}

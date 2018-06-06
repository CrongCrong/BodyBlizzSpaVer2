using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ProductForm.xaml
    /// </summary>
    public partial class ProductForm : MetroWindow
    {
        public ProductForm()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        ProductsModel productModel;
        ClientModel clientModel;
        ClientDetails clientDetails;
        string queryString = "";
        List<string> parameters;

        public ProductForm(ProductsModel pm)
        {
            productModel = pm;
            InitializeComponent();
        }

        public ProductForm(ClientDetails cd, ClientModel cm)
        {
            clientDetails = cd;
            clientModel = cm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            fillComboDiscount();
            fillComboWithProducts();
        }

        private void fillComboWithProducts()
        {
            conDB = new ConnectionDB();
            queryString = "SELECT ID, productName, description, price, stocks FROM dbspa.tblproducts WHERE isDeleted = 0;";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                ProductsModel prod = new ProductsModel();
                prod.ID = reader["ID"].ToString();
                prod.ProductName = reader["productName"].ToString();
                prod.Description = reader["description"].ToString();
                prod.Price = reader["price"].ToString();
                prod.Stocks = reader["stocks"].ToString();

                cmbProducts.Items.Add(prod);
                if (productModel != null)
                {
                    if (prod.ID.Equals(productModel.ID))
                    {
                        cmbProducts.SelectedItem = prod;
                    }
                }
            }


            conDB.closeConnection();

        }

        public void fillComboDiscount()
        {
            try
            {
                conDB = new ConnectionDB();
                queryString = "SELECT ID,discount, description FROM dbspa.tbldiscount WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    DiscountModel dm = new DiscountModel();
                    dm.ID1 = reader["ID"].ToString();
                    dm.Discount = reader["discount"].ToString();
                    dm.Description = reader["description"].ToString();

                    cmbDiscount.Items.Add(dm);
                }

                conDB.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private CODModel checkCODNumber()
        {
            CODModel codMod = new CODModel();

            try
            {
                conDB = new ConnectionDB();
                queryString = "SELECT ID, COD, dateServiced FROM dbspa.tblcod WHERE (dateServiced = ?)";

                parameters = new List<string>();

                DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
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

        private SerialNumberModel checkSerialNumber()
        {
            SerialNumberModel snm = new SerialNumberModel();
            try
            {
                conDB = new ConnectionDB();
                queryString = "SELECT ID, serialNumber FROM dbspa.tblserialnumber WHERE (ID =" +
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

        private bool insertSerialNumber(int serial)
        {
            bool ifInserted = false;
            try
            {
                conDB = new ConnectionDB();
                queryString = "INSERT INTO dbspa.tblserialnumber (serialNumber, dateServiced) VALUES(?, ?)";
                List<string> parameters = new List<string>();

                parameters.Add(serial.ToString());
                DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
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
                conDB = new ConnectionDB();
                queryString = "INSERT INTO dbspa.tblcod (COD, dateServiced)VALUES(?,?)";
                parameters = new List<string>();

                parameters.Add(cod.ToString());
                DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
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

        private bool updateCODNumber(int cod, int id)
        {
            bool ifUpdated = false;

            try
            {
                conDB = new ConnectionDB();
                queryString = "UPDATE dbspa.tblcod SET COD = ? WHERE ID = ?";
                parameters = new List<string>();

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

        private int insertClientRecord(ClientModel model)
        {
            int x = 0;
            try
            {
                string insert = "INSERT INTO dbspa.tblclient (dateServiced,serialNo,cod,firstName,lastName,address,servicemode,totalamt,timeIn,timeOut,isDeleted) " +
                    "VALUES(?,?,?,?,?,?,?,?,?,?,?)";
                parameters = new List<string>();

                DateTime date = DateTime.Parse(model.DateServiced);
                parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
                parameters.Add(model.SerialNumber);
                parameters.Add(model.Cod);
                parameters.Add(model.FirstName);
                parameters.Add(model.LastName);
                parameters.Add(model.Address);
                parameters.Add(model.ServiceMode);
                parameters.Add(" ");
                parameters.Add(model.TimeIn);
                parameters.Add(" ");
                parameters.Add(0.ToString());
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

        private ClientModel insertClientRecord()
        {
            //CODModel codMod = checkCODNumber();
            //SerialNumberModel serialMod = checkSerialNumber();

            //int codID = Convert.ToInt32(codMod.COD1);
            //int serial = Convert.ToInt32(serialMod.SerialNumber);

            //insertSerialNumber((serial + 1));
            //if (codID == 0)
            //{
            //    insertCODNumber((codID + 1));
            //    clientModel.Cod = (codID + 1).ToString();
            //    clientModel.SerialNumber = (serial + 1).ToString();
            //}
            //else
            //{
            //    updateCODNumber((codID + 1), Convert.ToInt32(codMod.ID1));
            //    clientModel.Cod = (codID + 1).ToString();
            //    clientModel.SerialNumber = (serial + 1).ToString();
            //}

            int x = insertClientRecord(clientModel);
            clientModel.ID1 = x.ToString();

            return clientModel;
        }

        private void updateClientRecordIfAlreadyExisting()
        {
            conDB = new ConnectionDB();
            queryString = "UPDATE dbspa.tblclient SET dateServiced = ? WHERE ID = ?";
            parameters = new List<string>();

            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            parameters.Add(clientModel.ID1);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private List<ProductBoughtModel> getProductsBoughtForClient(int id)
        {
            List<ProductBoughtModel> lstProductsBought = new List<ProductBoughtModel>();
            ProductBoughtModel prodBought = new ProductBoughtModel();
            conDB = new ConnectionDB();
            queryString = "SELECT dbspa.tblproductbought.ID, dbspa.tblproducts.productName, " +
                "IF(dbspa.tblproductbought.isDiscounted, (dbspa.tblproducts.price - dbspa.tblproducts.price * (dbspa.tbldiscount.discount / 100)) * dbspa.tblproductbought.totalqty, " +
                "dbspa.tblproducts.price * dbspa.tblproductbought.totalqty) AS 'PRICE', IF(dbspa.tblproductbought.isDiscounted, 'YES', 'NO') AS 'DISCOUNTED'" +
                " FROM(((dbspa.tblproductbought INNER JOIN dbspa.tblclient ON dbspa.tblproductbought.clientID = dbspa.tblclient.ID) " +
                "INNER JOIN dbspa.tblproducts ON dbspa.tblproductbought.productID = dbspa.tblproducts.ID) " +
                "INNER JOIN dbspa.tbldiscount ON dbspa.tblproductbought.discountID = dbspa.tbldiscount.ID) " +
                "WHERE dbspa.tblproductbought.isDeleted = 0 AND dbspa.tblproductbought.clientID = ?";

            parameters = new List<string>();
            parameters.Add(id.ToString());

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

        private void insertProductsBought(int id)
        {
            conDB = new ConnectionDB();
            queryString = "INSERT INTO dbspa.tblproductbought (productID, clientID, totalqty, datebought, isDiscounted, discountID, isDeleted) " +
                "VALUES (?,?,?,?,?,?,?)";
            parameters = new List<string>();

            DiscountModel dm = cmbDiscount.SelectedItem as DiscountModel;

            parameters.Add(cmbProducts.SelectedValue.ToString());
            parameters.Add(id.ToString());
            parameters.Add(txtQty.Text);

            DateTime date = DateTime.Parse(DateTime.Now.ToShortDateString());
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            int numDiscounted = 0;
            if (Convert.ToInt32(dm.Discount) > 0)
            {
                //ifDiscounted = true;
                numDiscounted = 1;
            }

            parameters.Add(numDiscounted.ToString());
            parameters.Add(dm.ID1);
            parameters.Add("0");

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("ADDED PRODUCTS FOR CLIENT ID: " + id + " PRODUCTS BOUGHT : " + cmbProducts.SelectedValue.ToString() + " QTY: " + txtQty.Text);
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;
            ProductsModel p = cmbProducts.SelectedItem as ProductsModel;
            DiscountModel d = cmbDiscount.SelectedItem as DiscountModel;

            if(p == null)
            {
                MessageBox.Show("Please select Product");

            }else if(d == null)
            {
                MessageBox.Show("Please select Discount");
            }else if(string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("Please input Qty (minimum of 1)");
            }
            else
            {
                ifAllCorrect = true;
            }


            return ifAllCorrect;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (clientModel.ID1 != "" && clientModel.ID1 != null)
            {
                if (checkFields())
                {
                    insertProductsBought(Convert.ToInt32(clientModel.ID1));
                    updateClientRecordIfAlreadyExisting();
                    clientDetails.dgvClientProduct.ItemsSource = getProductsBoughtForClient(Convert.ToInt32(clientModel.ID1));
                    MessageBox.Show("PRODUCT ADDED SUCCESSFULLY!");
                    
                    this.Close();
                }
                
            }
            else
            {
                if (checkFields())
                {
                    ClientModel c = insertClientRecord();
                    insertProductsBought(Convert.ToInt32(c.ID1));
                    clientDetails.ifAlreadySaved = true;
                    clientDetails.dgvClientProduct.ItemsSource = getProductsBoughtForClient(Convert.ToInt32(clientModel.ID1));
                    MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                    this.Close();
                }
                
            }
  
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }
    }
}

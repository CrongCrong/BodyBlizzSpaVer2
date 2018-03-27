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
    /// Interaction logic for ProductStocksDetails.xaml
    /// </summary>
    public partial class ProductStocksDetails : MetroWindow
    {
        public ProductStocksDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        ProductsBreakdown productsBreakdown;
        ProductsWindow productsWindow;
        ProductStocksModel prodStocksModel;

        public ProductStocksDetails(ProductsWindow pw)
        {
            productsWindow = pw;
            InitializeComponent();
        }

        public ProductStocksDetails(ProductsBreakdown pw, ProductStocksModel pm)
        {
            productsBreakdown = pw;
            prodStocksModel = pm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            fillComboProducts();
            btnUpdate.Visibility = Visibility.Hidden;
            if(prodStocksModel != null)
            {
                deliveryDate.Text = prodStocksModel.DeliveryDate;
                txtStocks.Text = prodStocksModel.Stocks;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private List<ProductBoughtModel> getProductsBought()
        {
            List<ProductBoughtModel> lstProductsBought = new List<ProductBoughtModel>();
            ProductBoughtModel pbm = new ProductBoughtModel();

            string queryString = "SELECT ID, productID, sum(totalqty) as 'TOTAL' FROM dbspa.tblproductbought WHERE isDeleted = 0 group by productID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                pbm.ID = reader["ID"].ToString();
                pbm.ProductID = reader["productID"].ToString();
                pbm.Total = reader["TOTAL"].ToString();
                lstProductsBought.Add(pbm);
                pbm = new ProductBoughtModel();
            }

            conDB.closeConnection();

            return lstProductsBought;
        }

        private void fillComboProducts()
        {

            string queryString = "SELECT ID, productName, description, price, stocks FROM dbspa.tblproducts WHERE isDeleted = 0;";

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
                if (prodStocksModel != null)
                {
                    if (prod.ID.Equals(prodStocksModel.ProductID))
                    {
                        cmbProducts.SelectedItem = prod;
                    }
                }
            }


            conDB.closeConnection();
        }

        private List<ProductStocksModel> loadProductStocksDataGridDetails()
        {
            List<ProductStocksModel> lstProductStocks = new List<ProductStocksModel>();
            ProductStocksModel ps = new ProductStocksModel();

            string queryString = "SELECT dbspa.tblproductstocks.ID, productID, date, SUM(dbspa.tblproductstocks.stocks) as st, dbspa.tblproducts.productName " +
                "FROM(dbspa.tblproductstocks INNER JOIN dbspa.tblproducts ON dbspa.tblproductstocks.productID = dbspa.tblproducts.ID) " +
                "WHERE dbspa.tblproductstocks.isDeleted = 0 GROUP BY productID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                ps.ID = reader["ID"].ToString();
                ps.ProductID = reader["productID"].ToString();
                ps.ProductName = reader["productName"].ToString();
                ps.Stocks = reader["st"].ToString();
                ps.DeliveryDate = reader["date"].ToString();
                lstProductStocks.Add(ps);
                ps = new ProductStocksModel();
            }

            conDB.closeConnection();

            foreach (ProductStocksModel psM in lstProductStocks)
            {
                foreach (ProductBoughtModel pbM in getProductsBought())
                {
                    if (psM.ProductID.Equals(pbM.ProductID))
                    {
                        psM.Stocks = (Convert.ToDouble(psM.Stocks) - Convert.ToDouble(pbM.Total)).ToString();
                    }
                }
            }

            return lstProductStocks;
        }

        private void loadProductsIn()
        {
            List<ProductStocksModel> lstProductsIn = new List<ProductStocksModel>();
            ProductStocksModel pIn = new ProductStocksModel();

            string queryString = "SELECT dbspa.tblproductstocks.ID, dbspa.tblproducts.productName, date, dbspa.tblproducts.ID as 'PRODUCT ID', dbspa.tblproductstocks.stocks " +
                "FROM(dbspa.tblproductstocks INNER JOIN dbspa.tblproducts ON dbspa.tblproductstocks.productID = dbspa.tblproducts.ID)";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                pIn.ID = reader["ID"].ToString();
                pIn.ProductName = reader["productName"].ToString();
                pIn.ProductID = reader["PRODUCT ID"].ToString();
                DateTime dte = DateTime.Parse(reader["date"].ToString());
                pIn.DeliveryDate = dte.ToShortDateString();
                pIn.Stocks = reader["stocks"].ToString();
                lstProductsIn.Add(pIn);
                pIn = new ProductStocksModel();

            }

            conDB.closeConnection();
            productsBreakdown.dgvProductsIn.ItemsSource = lstProductsIn;
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;
            ProductsModel p = cmbProducts.SelectedItem as ProductsModel;
            
            if(p == null)
            {
                MessageBox.Show("Please select product!");
            }else if (string.IsNullOrEmpty(txtStocks.Text))
            {
                MessageBox.Show("Please input stocks for product!");
            }else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void saveStocksProduct()
        {
            ProductsModel p = cmbProducts.SelectedItem as ProductsModel;
            string queryString = "INSERT INTO dbspa.tblproductstocks (productID, date, stocks, isDeleted) VALUES (?,?,?,?)";

            List<string> parameters = new List<string>();
            parameters.Add(p.ID);
            DateTime date = DateTime.Parse(deliveryDate.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(txtStocks.Text);
            parameters.Add("0");

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void updateStocksRecord()
        {
            ProductsModel p = cmbProducts.SelectedItem as ProductsModel;
            string queryString = "UPDATE dbspa.tblproductstocks SET productID = ?, date = ?, stocks = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(p.ID);
            DateTime date = DateTime.Parse(deliveryDate.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);
            parameters.Add(txtStocks.Text);
            parameters.Add(prodStocksModel.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                updateStocksRecord();
                loadProductsIn();
                MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                saveStocksProduct();
                productsWindow.dgvProductStocks.ItemsSource = loadProductStocksDataGridDetails();
                MessageBox.Show("RECORD ADDED SUCCESSFULLY!");
                this.Close();
            }
        }

        private void txtStocks_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

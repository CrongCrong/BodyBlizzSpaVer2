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
    /// Interaction logic for ProductDetails.xaml
    /// </summary>
    public partial class ProductDetails : MetroWindow
    {
        public ProductDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        ProductsWindow productsWindow;
        ProductsModel productModel;

        public ProductDetails(ProductsWindow pw)
        {
            productsWindow = pw;
            InitializeComponent();
        }

        public ProductDetails(ProductsWindow pw, ProductsModel pm)
        {
            productsWindow = pw;
            productModel = pm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if(productModel != null)
            {
                txtProductName.Text = productModel.ProductName;
                txtDescription.Text = productModel.Description;
                txtPrice.Text = productModel.Price;
                //txtStocks.Text = productModel.Stocks;

                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtProductName.Text))
            {
                MessageBox.Show("Please input Product Name!");
            } else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                MessageBox.Show("Please input Product Description!");
            } else if (string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Please input Product Price!");
            }else
            {
                ifAllCorrect = true;
            }


            return ifAllCorrect;
        }

        private void loadDataGridDetails()
        {
            List<ProductsModel> lstProducts = new List<ProductsModel>();
            ProductsModel product = new ProductsModel();

            string queryString = "SELECT ID, productName, description, price, stocks FROM dbspa.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                product.ID = reader["ID"].ToString();
                product.ProductName = reader["productName"].ToString();
                product.Description = reader["description"].ToString();
                product.Price = reader["price"].ToString();
                //product.Stocks = reader["stocks"].ToString();
                lstProducts.Add(product);
                product = new ProductsModel();

            }

            conDB.closeConnection();
            productsWindow.dgvProducts.ItemsSource = lstProducts;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(checkFields())
            {
                string queryString = "INSERT INTO dbspa.tblproducts (productName, description, price, " +
                    "isDeleted) VALUES (?,?,?,?)";
                List<string> parameters = new List<string>();

                parameters.Add(txtProductName.Text);
                parameters.Add(txtDescription.Text);
                parameters.Add(txtPrice.Text);
                //parameters.Add(txtStocks.Text);
                parameters.Add("0");

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                loadDataGridDetails();

                MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(checkFields())
            {
                string queryString = "UPDATE dbspa.tblproducts SET productName = ?, description = ?, price = ? " +
                    "WHERE ID = ?";

                List<string> parameters = new List<string>();
                parameters.Add(txtProductName.Text);
                parameters.Add(txtDescription.Text);
                parameters.Add(txtPrice.Text);
                //parameters.Add(txtStocks.Text);
                parameters.Add(productModel.ID);

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                loadDataGridDetails();

                MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                this.Close();

            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

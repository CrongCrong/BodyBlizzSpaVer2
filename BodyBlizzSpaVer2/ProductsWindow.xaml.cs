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
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : MetroWindow
    {
        public ProductsWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadProductDataGridDetails();
            dgvProductStocks.ItemsSource = loadProductStocksDataGridDetails();
        }

        private void loadProductDataGridDetails()
        {
            List<ProductsModel> lstProducts = new List<ProductsModel>();
            ProductsModel product = new ProductsModel();

            string queryString = "SELECT ID, productName, description, price, stocks FROM dbspa.tblproducts WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while(reader.Read())
            {
                product.ID = reader["ID"].ToString();
                product.ProductName = reader["productName"].ToString();
                product.Description = reader["description"].ToString();
                product.Price = reader["price"].ToString();
                product.Stocks = reader["stocks"].ToString();
                lstProducts.Add(product);
                product = new ProductsModel();

            }
            
            conDB.closeConnection();
            dgvProducts.ItemsSource = lstProducts;
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
                foreach(ProductBoughtModel pbM in getProductsBought())
                {
                    if (psM.ProductID.Equals(pbM.ProductID))
                    {
                        psM.Stocks = (Convert.ToDouble(psM.Stocks) - Convert.ToDouble(pbM.Total)).ToString();
                    }
                }
            }

            return lstProductStocks;
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

        private void deleteProductRecord(int id)
        {
            try
            {
                string queryString = "UPDATE dbspa.tblproducts SET isDeleted = ? WHERE ID = ?";
                List<string> parameters = new List<string>();
                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);

                loadProductDataGridDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductDetails prodDetails = new ProductDetails(this);
            prodDetails.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ProductsModel prodModel = dgvProducts.SelectedItem as ProductsModel;

            if (prodModel != null)
            {
                ProductDetails prodDetails = new ProductDetails(this, prodModel);
                prodDetails.ShowDialog();
            }
            else
            {
                MessageBox.Show("No records selected!");
            }

            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                ProductsModel cm = dgvProducts.SelectedItem as ProductsModel;

                if (cm != null)
                {
                    int id = Convert.ToInt32(cm.ID);

                    if (id != 0)
                    {
                        deleteProductRecord(id);
                        System.Windows.MessageBox.Show("Record deleted successfuly!");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No record selected!");
                }
            } 
        }

        private void btnAddProductStocks_Click(object sender, RoutedEventArgs e)
        {
            ProductStocksDetails prodStocks = new ProductStocksDetails(this);
            prodStocks.ShowDialog();
        }

        private void btnViewProductDetails_Click(object sender, RoutedEventArgs e)
        {
            ProductStocksModel ps = dgvProductStocks.SelectedItem as ProductStocksModel;

            if(ps != null)
            {
                ProductsBreakdown pb = new ProductsBreakdown(this, ps);
                pb.ShowDialog();
            }else
            {
                MessageBox.Show("Please select record!");
            }
            
        }
    }
}

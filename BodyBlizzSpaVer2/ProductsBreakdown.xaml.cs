using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ProductsBreakdown.xaml
    /// </summary>
    public partial class ProductsBreakdown : MetroWindow
    {
        public ProductsBreakdown()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        ProductsWindow productsWindow;
        ProductStocksModel productStocksModel;
        string queryString = "";
        List<string> parameters;

        public ProductsBreakdown(ProductsWindow pw)
        {
            productsWindow = pw;
            InitializeComponent();
        }

        public ProductsBreakdown(ProductsWindow pw, ProductStocksModel psm)
        {
            productStocksModel = psm;
            productsWindow = pw;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadProductsOut();
            loadProductsIn();
        }

        private void loadProductsOut()
        {
            List<ProductBoughtModel> lstProductsOut = new List<ProductBoughtModel>();
            ProductBoughtModel pb = new ProductBoughtModel();

            queryString = "SELECT dbspa.tblproducts.productName, CONCAT(dbspa.tblclient.firstName, ' ', dbspa.tblclient.lastName) as 'Whole Name', " +
                "totalqty FROM ((dbspa.tblproductbought INNER JOIN dbspa.tblclient ON dbspa.tblproductbought.clientID = dbspa.tblclient.ID) " +
                "INNER JOIN dbspa.tblproducts ON dbspa.tblproductbought.productID = dbspa.tblproducts.ID) WHERE dbspa.tblproductbought.isDeleted = 0 " +
                "AND dbspa.tblproductbought.productID = ?";
            parameters = new List<string>();
            parameters.Add(productStocksModel.ProductID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                pb.ProductName = reader["productName"].ToString();
                pb.ClientName = reader["Whole Name"].ToString();
                pb.Total = reader["totalqty"].ToString();
                lstProductsOut.Add(pb);
                pb = new ProductBoughtModel();
            }

            conDB.closeConnection();
            dgvProductsOut.ItemsSource = lstProductsOut;
        }

        private void loadProductsIn()
        {
            List<ProductStocksModel> lstProductsIn = new List<ProductStocksModel>();
            ProductStocksModel pIn = new ProductStocksModel();

            queryString = "SELECT dbspa.tblproductstocks.ID, dbspa.tblproducts.productName, date, dbspa.tblproducts.ID as 'PRODUCT ID', dbspa.tblproductstocks.stocks " +
                "FROM(dbspa.tblproductstocks INNER JOIN dbspa.tblproducts ON dbspa.tblproductstocks.productID = dbspa.tblproducts.ID) " +
                "WHERE dbspa.tblproductstocks.isDeleted = 0 AND dbspa.tblproductstocks.productID = ?";

            parameters = new List<string>();
            parameters.Add(productStocksModel.ProductID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

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
            dgvProductsIn.ItemsSource = lstProductsIn;
        }

        private List<ProductStocksModel> loadProductStocksDataGridDetails()
        {
            List<ProductStocksModel> lstProductStocks = new List<ProductStocksModel>();
            ProductStocksModel ps = new ProductStocksModel();

            queryString = "SELECT dbspa.tblproductstocks.ID, productID, date, SUM(dbspa.tblproductstocks.stocks) as st, dbspa.tblproducts.productName " +
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

        private List<ProductBoughtModel> getProductsBought()
        {
            List<ProductBoughtModel> lstProductsBought = new List<ProductBoughtModel>();
            ProductBoughtModel pbm = new ProductBoughtModel();

            queryString = "SELECT ID, productID, sum(totalqty) as 'TOTAL' FROM dbspa.tblproductbought WHERE isDeleted = 0 group by productID";

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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ProductStocksModel prodStocks = dgvProductsIn.SelectedItem as ProductStocksModel;

            if(prodStocks != null)
            {
                ProductStocksDetails prodDet = new ProductStocksDetails(this, prodStocks);
                prodDet.ShowDialog();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            productsWindow.dgvProductStocks.ItemsSource = loadProductStocksDataGridDetails();
        }
    }
}

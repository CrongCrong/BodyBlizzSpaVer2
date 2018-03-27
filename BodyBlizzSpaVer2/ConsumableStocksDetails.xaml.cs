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
    /// Interaction logic for ConsumableStocksDetails.xaml
    /// </summary>
    public partial class ConsumableStocksDetails : MetroWindow
    {
        public ConsumableStocksDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        ConsumableModel consumableStockModel;
        ConsumableWindow consumableWindow;

        public ConsumableStocksDetails(ConsumableWindow consWin, ConsumableModel cnsMod)
        {
            consumableStockModel = cnsMod;
            consumableWindow = consWin;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(consumableStockModel != null)
            {
                dgvConsumableOnStocks.ItemsSource = loadDatagridDetails(consumableStockModel.ID);
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            consumableWindow.dgvConsumableStocks.ItemsSource = loadConsumablesOnStocks();
        }

        private List<ConsumableModel> loadDatagridDetails(string consumableID)
        {
            List<ConsumableModel> lstConsumableOnStocks = new List<ConsumableModel>();
            ConsumableModel consumableStock = new ConsumableModel();

            string queryString = "SELECT dbspa.tblconsumableleft.ID, dbspa.tblconsumableleft.date, dbspa.tblconsumables.description, qty, used FROM " +
                "(dbspa.tblconsumableleft INNER JOIN dbspa.tblconsumables ON dbspa.tblconsumableleft.consumableID = " +
                "dbspa.tblconsumables.ID) WHERE dbspa.tblconsumableleft.isDeleted = 0 AND dbspa.tblconsumableleft.consumableID = ?;";

            List<string> parameters = new List<string>();
            parameters.Add(consumableID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                consumableStock.ID = reader["ID"].ToString();
                DateTime dte = DateTime.Parse(reader["date"].ToString());
                consumableStock.Date = dte.ToShortDateString();
                consumableStock.Description = reader["description"].ToString();
                consumableStock.Quantity = reader["qty"].ToString();
                consumableStock.Used = reader["used"].ToString();

                double dblQty = Convert.ToDouble(consumableStock.Quantity);
                double dblUsed = Convert.ToDouble(consumableStock.Used);

                consumableStock.Left = (dblQty - dblUsed).ToString();

                lstConsumableOnStocks.Add(consumableStock);

                consumableStock = new ConsumableModel();

            }

            conDB.closeConnection();
            return lstConsumableOnStocks;
        }

        private List<ConsumableModel> loadConsumablesOnStocks()
        {
            List<ConsumableModel> lstConsumablesStocks = new List<ConsumableModel>();
            ConsumableModel consumeStock = new ConsumableModel();

            string queryString = "Select dbspa.tblconsumableleft.consumableID, dbspa.tblconsumables.description, COUNT(*) as cnt FROM " +
                "(dbspa.tblconsumableleft INNER JOIN dbspa.tblconsumables ON dbspa.tblconsumableleft.consumableID = dbspa.tblconsumables.ID) " +
                "WHERE dbspa.tblconsumableleft.isDeleted = 0 GROUP BY dbspa.tblconsumableleft.consumableID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                consumeStock.ID = reader["consumableID"].ToString();
                consumeStock.Description = reader["description"].ToString();
                consumeStock.Count = reader["cnt"].ToString();
                lstConsumablesStocks.Add(consumeStock);
                consumeStock = new ConsumableModel();

            }

            return lstConsumablesStocks;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConsumableUpdates consumableUpdate = new ConsumableUpdates(this, consumableStockModel);
            consumableUpdate.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ConsumableModel consumMod = dgvConsumableOnStocks.SelectedItem as ConsumableModel;

            if(consumMod != null)
            {
                ConsumableUpdates consumableUpdate = new ConsumableUpdates(this, consumMod, consumableStockModel.ID, consumMod.ID);
                consumableUpdate.ShowDialog();
            }
        }

    }
}

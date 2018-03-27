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
    /// Interaction logic for ConsumableWindow.xaml
    /// </summary>
    public partial class ConsumableWindow : MetroWindow
    {
        public ConsumableWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgvConsumables.ItemsSource = loadConsumables();
            dgvConsumableStocks.ItemsSource = loadConsumablesOnStocks();
        }

        private List<ConsumableModel> loadConsumables()
        {

            List<ConsumableModel> lstConsumables = new List<ConsumableModel>();
            ConsumableModel consumables = new ConsumableModel();

            string queryString = "SELECT ID, name, description FROM dbspa.tblconsumables WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                consumables.ID = reader["ID"].ToString();
                consumables.Name = reader["name"].ToString();
                consumables.Description = reader["description"].ToString();
                lstConsumables.Add(consumables);
                consumables = new ConsumableModel();
            }

            conDB.closeConnection();

            return lstConsumables;
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
            ConsumableDetails consumDet = new ConsumableDetails(this);
            consumDet.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ConsumableModel consumableModel = dgvConsumables.SelectedItem as ConsumableModel;

            if(consumableModel != null)
            {
                ConsumableDetails consumDet = new ConsumableDetails(this, consumableModel);
                consumDet.ShowDialog();
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddConsumable_Click(object sender, RoutedEventArgs e)
        {
            ConsumableUpdates consumeUpdate = new ConsumableUpdates(this);
            consumeUpdate.ShowDialog();
        }

        private void btnViewConsumableDetails_Click(object sender, RoutedEventArgs e)
        {
            ConsumableModel consumableModel = dgvConsumableStocks.SelectedItem as ConsumableModel;

            if(consumableModel != null)
            {
                ConsumableStocksDetails csd = new ConsumableStocksDetails(this, consumableModel);
                csd.ShowDialog();

            }else
            {
                MessageBox.Show("No record selected!");
            }
        }
    }
}

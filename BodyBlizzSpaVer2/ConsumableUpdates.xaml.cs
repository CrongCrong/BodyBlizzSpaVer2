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
    /// Interaction logic for ConsumableUpdates.xaml
    /// </summary>
    public partial class ConsumableUpdates : MetroWindow
    {
        public ConsumableUpdates()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        ConsumableWindow consumableWindow;
        ConsumableStocksDetails consumableStockDetWindow;
        ConsumableModel consumeStockModel;
        string recordID = "";
        string consumableID = "";
        double dblQty = 0.0;
        double dblUsed = 0.0;


        public ConsumableUpdates(ConsumableStocksDetails csd, ConsumableModel cm)
        {
            consumableStockDetWindow = csd;
            consumeStockModel = cm;
            InitializeComponent();
        }

        public ConsumableUpdates(ConsumableStocksDetails csd, ConsumableModel cm, string consumeID, string recID)
        {
            consumableStockDetWindow = csd;
            consumeStockModel = cm;
            consumableID = consumeID;
            recordID = recID;
            InitializeComponent();
        }

        public ConsumableUpdates(ConsumableWindow consumWin)
        {
            consumableWindow = consumWin;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            loadConsumableList();
            if(consumeStockModel != null && !string.IsNullOrEmpty(recordID))
            {
                txtQty.IsEnabled = false;
                dateBought.IsEnabled = false;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
                dateBought.Text = consumeStockModel.Date;
                txtQty.Text = consumeStockModel.Quantity;
                txtUsed.Text = consumeStockModel.Used;
                dblQty = Convert.ToDouble(consumeStockModel.Quantity);
                dblUsed = Convert.ToDouble(consumeStockModel.Used);

                lblLeft.Content = (dblQty - dblUsed).ToString();

            }
            if (!string.IsNullOrEmpty(recordID))
            {
                cmbConsumables.IsEnabled = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(consumableWindow != null)
            {
                if (checkFields())
                {
                    saveConsumable();
                    consumableWindow.dgvConsumableStocks.ItemsSource = loadConsumablesOnStocks();
                    MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                    this.Close();
                }
            }

            if(consumableStockDetWindow != null)
            {
                saveConsumable();
                consumableStockDetWindow.dgvConsumableOnStocks.ItemsSource = loadDatagridDetails(consumeStockModel.ID);
                MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                this.Close();
            }
            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (consumableStockDetWindow != null)
            {
                if (checkFields())
                {


                    updateConsumableRecord();
                    consumableStockDetWindow.dgvConsumableOnStocks.ItemsSource = loadDatagridDetails(consumableID);
                    MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                    this.Close();
                }
            }

            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if(cmbConsumables.SelectedItem == null)
            {
                MessageBox.Show("Pleas select consumable item!");
            }else if (string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("Please input quantity!");
            }else if (string.IsNullOrEmpty(txtUsed.Text))
            {
                MessageBox.Show("Please input USED for consumable item!");
            }else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
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
                consumableStock.Date = reader["date"].ToString();
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

        private void loadConsumableList()
        {
            ConsumableModel consumables = new ConsumableModel();

            string queryString = "SELECT ID, name, description FROM dbspa.tblconsumables WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                consumables.ID = reader["ID"].ToString();
                consumables.Name = reader["name"].ToString();
                consumables.Description = reader["description"].ToString();

                cmbConsumables.Items.Add(consumables);
                if(!string.IsNullOrEmpty(consumableID))
                {
                    if (consumableID.Equals(consumables.ID))
                    {
                        cmbConsumables.SelectedItem = consumables;
                        cmbConsumables.IsEnabled = false;
                    }
                }else
                {
                    if (consumeStockModel != null)
                    {
                        if (consumeStockModel.ID.Equals(consumables.ID))
                        {
                            cmbConsumables.SelectedItem = consumables;
                            cmbConsumables.IsEnabled = false;
                        }
                    }
                }
                
                consumables = new ConsumableModel();
            }

            conDB.closeConnection();

        }

        private void saveConsumable()
        {

            string queryString = "INSERT INTO dbspa.tblconsumableleft (date, consumableID, qty, used, isDeleted) VALUES (?,?,?,?,?)";

            List<string> parameters = new List<string>();
            DateTime dte = DateTime.Parse(dateBought.Text);
            parameters.Add(dte.Year + "-" + dte.Month + "-" + dte.Day);
            parameters.Add(cmbConsumables.SelectedValue.ToString());
            parameters.Add(txtQty.Text);
            parameters.Add(txtUsed.Text);
            parameters.Add("0");

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void updateConsumableRecord()
        {
            string queryString = "UPDATE dbspa.tblconsumableleft SET qty = ?, used = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            dblUsed = dblUsed + Convert.ToDouble(txtUsed.Text);
            parameters.Add(txtQty.Text);
            parameters.Add(dblUsed.ToString());
            parameters.Add(recordID);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();

        }

    }
}

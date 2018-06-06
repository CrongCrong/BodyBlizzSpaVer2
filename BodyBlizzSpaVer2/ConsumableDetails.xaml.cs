using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ConsumableDetails.xaml
    /// </summary>
    public partial class ConsumableDetails : MetroWindow
    {
        public ConsumableDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        ConsumableWindow consumableWin;
        ConsumableModel consumableMod;
        List<string> parameters;
        string queryString = "";

        public ConsumableDetails(ConsumableWindow consumWin)
        {
            consumableWin = consumWin;
            InitializeComponent();
        }

        public ConsumableDetails(ConsumableWindow consumWin, ConsumableModel consumMod)
        {
            consumableWin = consumWin;
            consumableMod = consumMod;
            InitializeComponent();
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;

            if (consumableMod != null)
            {
                btnUpdate.Visibility = Visibility.Visible;
                txtName.Text = consumableMod.Name;
                txtDescription.Text = consumableMod.Description;
                btnSave.Visibility = Visibility.Hidden;
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                saveConsumable();
                MessageBox.Show("Record added successfully!");
                consumableWin.dgvConsumables.ItemsSource = loadConsumables();
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                updateConsumable();
                MessageBox.Show("Record updated successfully!");
                consumableWin.dgvConsumables.ItemsSource = loadConsumables();
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please input Name");
            }else if (string.IsNullOrEmpty(txtDescription.Text))
            {
                MessageBox.Show("Please input Description");
            }else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private List<ConsumableModel> loadConsumables()
        {

            List<ConsumableModel> lstConsumables = new List<ConsumableModel>();
            ConsumableModel consumables = new ConsumableModel();

            queryString = "SELECT ID, name, description FROM dbspa.tblconsumables WHERE isDeleted = 0";

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

        private void saveConsumable()
        {

            queryString = "INSERT INTO dbspa.tblconsumables (name, description, isDeleted) VALUES (?,?,0)";

            parameters = new List<string>();

            parameters.Add(txtName.Text);
            parameters.Add(txtDescription.Text);

            conDB.getSelectConnection(queryString, parameters);

            conDB.closeConnection();

        }

        private void updateConsumable()
        {

            queryString = "UPDATE dbspa.tblconsumables SET name = ?, description = ? WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(txtName.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(consumableMod.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();


        }
    }
}

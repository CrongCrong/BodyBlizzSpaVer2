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
    /// Interaction logic for LoyalCardDetails.xaml
    /// </summary>
    public partial class LoyalCardDetails : MetroWindow
    {
        public LoyalCardDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        LoyaltyCardWindow loyaltyWindow;
        LoyaltyCardModel loyaltyCard;

        public LoyalCardDetails(LoyaltyCardWindow lcw)
        {
            loyaltyWindow = lcw;
            InitializeComponent();
        }

        public LoyalCardDetails(LoyaltyCardWindow lcw, LoyaltyCardModel lcm)
        {
            loyaltyWindow = lcw;
            loyaltyCard = lcm;
            InitializeComponent();
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if (loyaltyCard != null)
            {
                txtSerialNumber.Text = loyaltyCard.SerialNumber;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
            }
        }

        private void loadDataGridDetails()
        {
            List<LoyaltyCardModel> lstLoyaltyCard = new List<LoyaltyCardModel>();
            LoyaltyCardModel loyalty = new LoyaltyCardModel();

            string queryString = "SELECT dbspa.tblloyaltycard.ID, dbspa.tblclient.ID AS 'clientID', serialnumber,CONCAT(dbspa.tblclient.firstName, ' ', dbspa.tblclient.lastName) as 'Whole Name' " +
                "FROM (dbspa.tblloyaltycard INNER JOIN dbspa.tblclient ON dbspa.tblloyaltycard.clientID = dbspa.tblclient.ID)";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                loyalty.ID = reader["ID"].ToString();
                loyalty.ClientID = reader["clientID"].ToString();
                loyalty.SerialNumber = reader["serialnumber"].ToString();
                loyalty.ClientName = reader["Whole Name"].ToString();
                lstLoyaltyCard.Add(loyalty);
                loyalty = new LoyaltyCardModel();             
            }
            conDB.closeConnection();
            lstLoyaltyCard.AddRange(getCardWithNoClient());          
            loyaltyWindow.dgvLoyaltyCard.ItemsSource = lstLoyaltyCard;
        }

        private List<LoyaltyCardModel> getCardWithNoClient()
        {
            List<LoyaltyCardModel> lstLoyaltyCard = new List<LoyaltyCardModel>();
            LoyaltyCardModel loyalty = new LoyaltyCardModel();

            string queryString = "SELECT dbspa.tblloyaltycard.ID, serialnumber FROM dbspa.tblloyaltycard WHERE dbspa.tblloyaltycard.clientID = 0";
            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            while (reader.Read())
            {
                loyalty.ID = reader["ID"].ToString();
                loyalty.SerialNumber = reader["serialnumber"].ToString();
                loyalty.ClientName = "";
                lstLoyaltyCard.Add(loyalty);
                loyalty = new LoyaltyCardModel();
            }
            conDB.closeConnection();

            return lstLoyaltyCard;
        }

        private void saveLoyaltyCard()
        {
            string queryString = "INSERT INTO dbspa.tblloyaltycard (serialnumber, isDeleted) VALUES (?,?)";
            List<string> parameters = new List<string>();
            parameters.Add(txtSerialNumber.Text);
            parameters.Add("0");
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void updateLoyaltyCard()
        {
            string queryString = "UPDATE dbspa.tblloyaltycard SET serialnumber = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtSerialNumber.Text);
            parameters.Add(loyaltyCard.ID);
            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerialNumber.Text))
            {
                saveLoyaltyCard();
                loadDataGridDetails();
                MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Please input Serial Number!");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerialNumber.Text))
            {
                updateLoyaltyCard();
                loadDataGridDetails();
                MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Please input Serial Number!");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

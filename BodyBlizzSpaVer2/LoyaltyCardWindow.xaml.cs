using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for LoyaltyCardWindow.xaml
    /// </summary>
    public partial class LoyaltyCardWindow : MetroWindow
    {
        public LoyaltyCardWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        string queryString = "";
        List<string> parameters;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadDataGridDetails();
        }

        private void loadDataGridDetails()
        {
            List<LoyaltyCardModel> lstLoyaltyCard = new List<LoyaltyCardModel>();
            LoyaltyCardModel loyalty = new LoyaltyCardModel();

            queryString = "SELECT dbspa.tblloyaltycard.ID, dbspa.tblclient.ID AS 'clientID', serialnumber,CONCAT(dbspa.tblclient.firstName, ' ', dbspa.tblclient.lastName) as 'Whole Name' " +
                "FROM (dbspa.tblloyaltycard INNER JOIN dbspa.tblclient ON dbspa.tblloyaltycard.clientID = dbspa.tblclient.ID) WHERE dbspa.tblloyaltycard.isDeleted = 0";

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
            dgvLoyaltyCard.ItemsSource = lstLoyaltyCard;
        }

        private List<LoyaltyCardModel> getCardWithNoClient()
        {
            List<LoyaltyCardModel> lstLoyaltyCard = new List<LoyaltyCardModel>();
            LoyaltyCardModel loyalty = new LoyaltyCardModel();


            queryString = "SELECT dbspa.tblloyaltycard.ID, serialnumber FROM dbspa.tblloyaltycard WHERE dbspa.tblloyaltycard.clientID = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while(reader.Read())
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

        private void deleteRecord(string recID)
        {
            queryString = "UPDATE dbspa.tblloyaltycard SET isDeleted = 1 WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void removeLoyaltyCardFromClient(string recID)
        {
            queryString = "UPDATE dbspa.tblclient SET isLoyal = 0, LoyaltyID = 0 WHERE LoyaltyID = ?";

            parameters = new List<string>();
            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            LoyalCardDetails lcd = new LoyalCardDetails(this);
            lcd.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            LoyaltyCardModel loyalty = dgvLoyaltyCard.SelectedItem as LoyaltyCardModel;

           if(loyalty != null)
            {
                LoyalCardDetails lcd = new LoyalCardDetails(this, loyalty);
                lcd.ShowDialog();
            }else
            {
                System.Windows.MessageBox.Show("No record selected!");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                LoyaltyCardModel lc = dgvLoyaltyCard.SelectedItem as LoyaltyCardModel;

                if(lc != null)
                {
                    deleteRecord(lc.ID);
                    if (!string.IsNullOrEmpty(lc.ClientName))
                    {
                        removeLoyaltyCardFromClient(lc.ID);
                    }
                    
                    loadDataGridDetails();

                    System.Windows.MessageBox.Show("RECORD DELETED SUCCESSFULLY!");
                }
            }
        }
    }
}

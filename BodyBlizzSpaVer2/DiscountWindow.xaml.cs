using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for DiscountWindow.xaml
    /// </summary>
    public partial class DiscountWindow : MetroWindow
    {
        public DiscountWindow()
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
            List<DiscountModel> lstDiscounts = new List<DiscountModel>();
            DiscountModel ds = new DiscountModel();

            queryString = "SELECT ID, discount as 'DISCOUNT (%)', description AS 'DESCRIPTION' FROM dbspa.tbldiscount WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                ds.ID1 = reader["ID"].ToString();
                ds.Discount = reader["DISCOUNT (%)"].ToString();
                ds.Description = reader["DESCRIPTION"].ToString();
                lstDiscounts.Add(ds);
                ds = new DiscountModel();
            }

            dgvDiscounts.ItemsSource = lstDiscounts;
            conDB.closeConnection();

        }

        private void deleteRecord(int id)
        {
            try
            {
                queryString = "UPDATE dbspa.tbldiscount SET isDeleted = ? WHERE ID = ?";
                parameters = new List<string>();
                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                loadDataGridDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DiscountDetails dd = new DiscountDetails(this);
            dd.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            DiscountModel dm = dgvDiscounts.SelectedItem as DiscountModel;

            if (dm != null)
            {
                DiscountDetails dd = new DiscountDetails(this, dm);
                dd.ShowDialog();
            }
            else
            {
                MessageBox.Show("No record selected.");
            }          

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                DiscountModel dm = dgvDiscounts.SelectedItem as DiscountModel;

                if (dm != null)
                {
                    int id = Convert.ToInt32(dm.ID1);

                    if (id != 0)
                    {
                        deleteRecord(id);
                        MessageBox.Show("Record deleted successfuly!");
                    }
                }
                else
                {
                    MessageBox.Show("No record selected!");
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

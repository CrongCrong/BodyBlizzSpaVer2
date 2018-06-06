using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for CommissionWindow.xaml
    /// </summary>
    public partial class CommissionWindow : MetroWindow
    {
        public CommissionWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        string queryString = "";
        List<string> parameters;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getDatagridDetails();
        }

        private void getPromoWithCommissions()
        {

        }

        public void getDatagridDetails()
        {
            List<CommissionView> lstCommissions = new List<CommissionView>();
            CommissionView commissionView = new CommissionView();

            try
            {

                queryString = "SELECT dbspa.tblcommissions.ID, dbspa.tblservicetype.description as 'SERVICE TYPE', " +
                    "dbspa.tblcommissions.commission as 'COMMISSION' FROM (dbspa.tblcommissions INNER JOIN dbspa.tblservicetype ON " +
                    "dbspa.tblcommissions.serviceTypeID = dbspa.tblservicetype.ID) WHERE (dbspa.tblcommissions.isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    commissionView.ID = reader["ID"].ToString();
                    commissionView.ServiceType = reader["SERVICE TYPE"].ToString();
                    commissionView.Commission = reader["COMMISSION"].ToString();
                    lstCommissions.Add(commissionView);
                    commissionView = new CommissionView();
                }

                conDB.closeConnection();

                dgvCommission.ItemsSource = lstCommissions;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private CommissionModel getCommissionModel(int id)
        {
            CommissionModel comMod = new CommissionModel();
            try
            {
                queryString = "SELECT ID, serviceTypeID, commission FROM dbspa.tblcommissions WHERE (isDeleted = 0) AND (ID = ?)";

                parameters = new List<string>();
                parameters.Add(id.ToString());

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

                while (reader.Read())
                {
                    comMod.ServiceTypeID = reader["serviceTypeID"].ToString();
                    comMod.Commission = reader["commission"].ToString();
                    comMod.ID1 = reader["ID"].ToString();
                }

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return comMod;
        }

        private ServiceTypeModel getServiceTypeModel(int id)
        {
            ServiceTypeModel stm = new ServiceTypeModel();

            try
            {
                queryString = "SELECT ID, serviceType AS 'SERVICE TYPE', price AS 'PRICE' FROM dbspa.tblservicetype WHERE (isDeleted = 0) AND (ID = ?)";
                parameters = new List<string>();
                parameters.Add(id.ToString());

                MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);
                while (reader.Read())
                {
                    stm.ServiceType = reader["SERVICE TYPE"].ToString();
                    stm.Price = reader["price"].ToString();
                    stm.ID1 = reader["ID"].ToString();
                }
                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return stm;
        }

        private void deleteClientRecord(int id)
        {

            try
            {
                queryString = "UPDATE dbspa.tblcommissions SET isDeleted = ? WHERE ID = ?";
                parameters = new List<string>();
                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CommissionDetails std = new CommissionDetails(this);
            std.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            CommissionView comView = dgvCommission.SelectedItem as CommissionView;
            
            if (comView != null)
            {
                int id = Convert.ToInt32(comView.ID);
                if (id != 0)
                {
                    CommissionModel com = getCommissionModel(id);
                    com.ServiceTypeModel = getServiceTypeModel(Convert.ToInt32(com.ServiceTypeID));
                    com.IfEditDetails = true;

                    CommissionDetails cd = new CommissionDetails(this, com);
                    cd.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("No record selected!");
            }           
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                CommissionView cm = dgvCommission.SelectedItem as CommissionView;

                if (cm != null)
                {
                    int id = Convert.ToInt32(cm.ID);

                    if (id != 0)
                    {
                        deleteClientRecord(id);
                        getDatagridDetails();
                        System.Windows.MessageBox.Show("Record deleted successfuly!");

                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No record selected!");
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }



}

﻿using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ServiceTypeWindow.xaml
    /// </summary>
    public partial class ServiceTypeWindow : MetroWindow
    {
        public ServiceTypeWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        string queryString = "";
        List<string> parameters;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadDataGridDetails();
        }

        private void loadDataGridDetails()
        {
            List<ServiceTypeModel> lstServiceType = new List<ServiceTypeModel>();
            ServiceTypeModel serviceModel = new ServiceTypeModel();

            queryString = "SELECT ID, serviceType AS 'SERVICE TYPE', price AS 'PRICE' , " +
                "description AS 'DESCRIPTION' FROM dbspa.tblservicetype WHERE (isDeleted = 0) order by servicetype asc";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                serviceModel.ID1 = reader["ID"].ToString();
                serviceModel.ServiceType = reader["SERVICE TYPE"].ToString();
                serviceModel.Price = reader["PRICE"].ToString();
                serviceModel.Description = reader["DESCRIPTION"].ToString();
                lstServiceType.Add(serviceModel);
                serviceModel = new ServiceTypeModel();
            }
            dgvServiceType.ItemsSource = lstServiceType;
            conDB.closeConnection();

        }

        private void deleteRecord(int id)
        {
            try
            {
                queryString = "UPDATE dbspa.tblservicetype SET isDeleted = ? WHERE ID = ?";
                parameters = new List<string>();
                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);


                loadDataGridDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ServiceTypeDetails std = new ServiceTypeDetails(this);
            std.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ServiceTypeModel stm = dgvServiceType.SelectedItem as ServiceTypeModel;

            if(stm != null)
            {
                ServiceTypeDetails std = new ServiceTypeDetails(this, stm);
                std.ShowDialog();
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
                ServiceTypeModel sm = dgvServiceType.SelectedItem as ServiceTypeModel;

                if (sm != null)
                {
                    int id = Convert.ToInt32(sm.ID1);

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

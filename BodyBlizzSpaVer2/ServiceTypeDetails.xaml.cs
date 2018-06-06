using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for ServiceTypeDetails.xaml
    /// </summary>
    public partial class ServiceTypeDetails : MetroWindow
    {
        public ServiceTypeDetails()
        {
            InitializeComponent();
        }

        ServiceTypeModel serviceTypeModel;
        ConnectionDB conDB = new ConnectionDB();
        ServiceTypeWindow serviceTypeWindow;
        string queryString = "";
        List<string> parameters;

        public ServiceTypeDetails(ServiceTypeWindow st)
        {
            serviceTypeWindow = st;
            InitializeComponent();
        }

        public ServiceTypeDetails(ServiceTypeWindow st, ServiceTypeModel  stm)
        {
            serviceTypeModel = stm;
            serviceTypeWindow = st;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if (serviceTypeModel != null)
            {
                txtServiceType.Text = serviceTypeModel.ServiceType;
                txtPrice.Text = serviceTypeModel.Price;
                txtDescription.Text = serviceTypeModel.Description;
                btnUpdate.Visibility = Visibility.Visible;
                btnAdd.Visibility = Visibility.Hidden;
            }
        }

        private bool checkFields()
        {
            bool ifCorrect = false;

            if (string.IsNullOrEmpty(txtServiceType.Text))
            {
                MessageBox.Show("Please input value for Servie Type!");
            }
            else if (string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Please input value for Price!");
            }
            else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private void loadDataGridDetails()
        {
            List<ServiceTypeModel> lstServiceType = new List<ServiceTypeModel>();
            ServiceTypeModel serviceModel = new ServiceTypeModel();

            queryString = "SELECT ID, serviceType AS 'SERVICE TYPE', price AS 'PRICE' , description AS 'DESCRIPTION' FROM dbspa.tblservicetype WHERE (isDeleted = 0)";

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
            serviceTypeWindow.dgvServiceType.ItemsSource = lstServiceType;
            conDB.closeConnection();
           
        }

        private void updateServiceTypeDetails(ServiceTypeModel serviceTypeModel)
        {

            queryString = "UPDATE dbspa.tblservicetype SET serviceType = ?, description = ?, price = ? " +
                "WHERE ID = ?";
            parameters = new List<string>();

            parameters.Add(txtServiceType.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(txtPrice.Text);
            parameters.Add(serviceTypeModel.ID1);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

            loadDataGridDetails();

            MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");


            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkFields())
                {
                    queryString = "INSERT INTO dbspa.tblservicetype (serviceType, price, description, isDeleted)" +
                        "VALUES(?,?,?,?)";
                    parameters = new List<string>();

                    parameters.Add(txtServiceType.Text);
                    parameters.Add(txtPrice.Text);
                    parameters.Add(txtDescription.Text);
                    parameters.Add(0.ToString());

                    conDB.AddRecordToDatabase(queryString, parameters);
                    conDB.closeConnection();


                    loadDataGridDetails();
                    MessageBox.Show("RECORD SAVED SUCCESSFULLY!");

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkFields())
                {
                    updateServiceTypeDetails(serviceTypeModel);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }
    }
}

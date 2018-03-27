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
    /// Interaction logic for ServiceModeDetails.xaml
    /// </summary>
    public partial class ServiceModeDetails : MetroWindow
    {
        public ServiceModeDetails()
        {
            InitializeComponent();
        }

        
        ConnectionDB conDB = new ConnectionDB();
        ServiceModeWindow serviceModeWindow;
        ServiceModeModel serviceModel;
        public ServiceModeDetails(ServiceModeWindow sm)
        {
            serviceModeWindow = sm;
            InitializeComponent();
        }

        public ServiceModeDetails(ServiceModeWindow sm, ServiceModeModel smm)
        {
            serviceModel = smm;
            serviceModeWindow = sm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if (serviceModel != null)
            {
                if (serviceModel.IfEditDetails)
                {
                    txtServiceMode.Text = serviceModel.ServiceType;
                    btnUpdate.Visibility = Visibility.Visible;
                }
            }
        }

        private bool checkFields()
        {
            bool ifCorrect = false;
            if (string.IsNullOrEmpty(txtServiceMode.Text))
            {
                MessageBox.Show("Please input value!");

            }
            else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private void loadDataGridDetails()
        {
            List<ServiceModeModel> lstServiceMode = new List<ServiceModeModel>();
            ServiceModeModel serviceMode = new ServiceModeModel();
            string queryString = "SELECT ID, serviceType AS 'SERVICE TYPE' FROM dbspa.tblservicemode WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                serviceMode.ID1 = reader["ID"].ToString();
                serviceMode.ServiceType = reader["SERVICE TYPE"].ToString();
                lstServiceMode.Add(serviceMode);
                serviceMode = new ServiceModeModel();
            }

            conDB.closeConnection();
            serviceModeWindow.dgvServiceMode.ItemsSource = lstServiceMode;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkFields())
                {

                    string queryString = "INSERT INTO dbspa.tblservicemode (serviceType, isDeleted)" +
                        "VALUES(?,?)";

                    List<string> parameters = new List<string>();
                    parameters.Add(txtServiceMode.Text);
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
                string queryString = "UPDATE dbspa.tblservicemode SET serviceType = ? WHERE ID = ?";
                List<string> parameters = new List<string>();

                parameters.Add(txtServiceMode.Text);
                parameters.Add(serviceModel.ID1);

                conDB.AddRecordToDatabase(queryString, parameters);
    

                loadDataGridDetails();

                MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

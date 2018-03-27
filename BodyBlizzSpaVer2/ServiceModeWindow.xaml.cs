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
    /// Interaction logic for ServiceModeWindow.xaml
    /// </summary>
    public partial class ServiceModeWindow : MetroWindow
    {
        public ServiceModeWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getDatagridDetails();
        }

        public void getDatagridDetails()
        {
            List<ServiceModeModel> lstServiceMode = new List<ServiceModeModel>();
            ServiceModeModel service = new ServiceModeModel();
            try
            {
                string queryString = "SELECT ID, serviceType AS 'SERVICE TYPE' FROM dbspa.tblservicemode WHERE isDeleted = 0";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    service.ID1 = reader["ID"].ToString();
                    service.ServiceType = reader["SERVICE TYPE"].ToString();
                    lstServiceMode.Add(service);
                    service = new ServiceModeModel();
                }

                dgvServiceMode.ItemsSource = lstServiceMode;
                conDB.closeConnection();
   
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteClientRecord(int id)
        {
            try
            {
                string queryString = "UPDATE dbspa.tblservicemode SET isDeleted = ? WHERE ID = ?";
                List<string> parameters = new List<string>();

                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                getDatagridDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ServiceModeDetails cd = new ServiceModeDetails(this);
            cd.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ServiceModeModel smm = dgvServiceMode.SelectedItem as ServiceModeModel;

            if(smm != null)
            {

                int id = Convert.ToInt32(smm.ID1);
                smm.IfEditDetails = true;

                ServiceModeDetails cd = new ServiceModeDetails(this, smm);
                cd.ShowDialog();

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
                ServiceModeModel sm = dgvServiceMode.SelectedItem as ServiceModeModel;

                if (sm != null)
                {
                    int id = Convert.ToInt32(sm.ID1);

                    if (id != 0)
                    {
                        deleteClientRecord(id);
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

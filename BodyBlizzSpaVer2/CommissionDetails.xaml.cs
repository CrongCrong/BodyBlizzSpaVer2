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
    /// Interaction logic for CommissionDetails.xaml
    /// </summary>
    public partial class CommissionDetails : MetroWindow
    {
        public CommissionDetails()
        {
            InitializeComponent();
        }

        CommissionWindow comms;
        CommissionModel comModel;
        ConnectionDB conDB = new ConnectionDB();

        public CommissionDetails(CommissionWindow co)
        {
            comms = co;
            InitializeComponent();
        }
        public CommissionDetails(CommissionWindow co, CommissionModel cm)
        {
            comModel = cm;
            comms = co;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;

            if (comModel != null)
            {
                if (comModel.IfEditDetails)
                {
                    fillListBoxServiceType(cmbServiceType, comModel.ServiceTypeModel);
                    cmbServiceType.IsEnabled = false;
                    txtCommission.Text = comModel.Commission;
                    btnAdd.Visibility = Visibility.Hidden;
                    btnUpdate.Visibility = Visibility.Visible;

                }
                else
                {
                    fillListBoxServiceType(cmbServiceType);
                }
            }
            else
            {
                fillListBoxServiceType(cmbServiceType);
            }
        }

        public void fillListBoxServiceType(ComboBox combo)
        {

            //GET SERVICE with COMMISION THEN REMOVE FROM LIST
            List<ServiceTypeModel> lstServiceType = getServiceWithCommissions();
            try
            {

                string queryString = "SELECT dbspa.tblservicetype.ID, dbspa.tblservicetype.serviceType AS 'SERVICE TYPE', dbspa.tblservicetype.price AS 'PRICE', " +
                    "description FROM dbspa.tblservicetype WHERE (dbspa.tblservicetype.isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    ServiceTypeModel stm = new ServiceTypeModel();
                    stm.ServiceType = reader["SERVICE TYPE"].ToString();
                    stm.Price = reader["price"].ToString();
                    stm.ID1 = reader["ID"].ToString();
                    stm.Description = reader["description"].ToString();

                    combo.Items.Add(stm);
                    foreach (ServiceTypeModel t in lstServiceType)
                    {
                        if (stm.ID1.Equals(t.ID1))
                        {
                            combo.Items.Remove(stm);
                        }
                    }
                }
                
                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void fillListBoxServiceType(ComboBox combo, ServiceTypeModel servType)
        {
            try
            {
                string queryString = "SELECT dbspa.tblservicetype.ID, dbspa.tblservicetype.serviceType AS 'SERVICE TYPE', dbspa.tblservicetype.price AS 'PRICE', " +
                    "description FROM dbspa.tblservicetype WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    ServiceTypeModel stm = new ServiceTypeModel();
                    stm.ServiceType = reader["SERVICE TYPE"].ToString();
                    stm.Price = reader["price"].ToString();
                    stm.ID1 = reader["ID"].ToString();
                    stm.Description = reader["description"].ToString();
                    combo.Items.Add(stm);

                    if (stm.ID1.Equals(servType.ID1))
                    {
                        combo.SelectedItem = stm;
                    }
                }

                conDB.closeConnection();      

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private List<ServiceTypeModel> getServiceWithCommissions()
        {
            List<ServiceTypeModel> lstServiceModel = new List<ServiceTypeModel>();
            try
            {

                string queryString = "SELECT dbspa.tblservicetype.ID, dbspa.tblservicetype.serviceType AS 'SERVICE TYPE', dbspa.tblservicetype.price FROM " +
                    "(dbspa.tblservicetype INNER JOIN dbspa.tblcommissions ON dbspa.tblservicetype.ID = dbspa.tblcommissions.serviceTypeID) WHERE " +
                    "(dbspa.tblservicetype.isDeleted = 0) AND (dbspa.tblcommissions.isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    ServiceTypeModel stm = new ServiceTypeModel();
                    stm.ServiceType = reader["SERVICE TYPE"].ToString();
                    stm.Price = reader["price"].ToString();
                    stm.ID1 = reader["ID"].ToString();
                    lstServiceModel.Add(stm);
                }
                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return lstServiceModel;
        }

        public void getDatagridDetails()
        {
            List<CommissionView> lstCommissions = new List<CommissionView>();
            CommissionView commissionView = new CommissionView();

            try
            {

                string queryString = "SELECT dbspa.tblcommissions.ID, dbspa.tblservicetype.description as 'SERVICE TYPE', " + 
                    "dbspa.tblcommissions.commission as 'COMMISSION' FROM (dbspa.tblcommissions INNER JOIN dbspa.tblservicetype ON " + 
                    "dbspa.tblcommissions.serviceTypeID = dbspa.tblservicetype.ID) WHERE (dbspa.tblcommissions.isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while(reader.Read())
                {
                    commissionView.ID = reader["ID"].ToString();
                    commissionView.ServiceType = reader["SERVICE TYPE"].ToString();
                    commissionView.Commission = reader["COMMISSION"].ToString();
                    lstCommissions.Add(commissionView);
                    commissionView = new CommissionView();
                }

                conDB.closeConnection();

                comms.dgvCommission.ItemsSource = lstCommissions;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateCommissionDetails(CommissionModel comModel)
        {
            try
            {

                CommissionModel stm = cmbServiceType.SelectedItem as CommissionModel;

                string queryString = "UPDATE dbspa.tblcommissions SET commission = ? " +
                    "WHERE ID = ?";
                List<string> parameters = new List<string>();
                parameters.Add(String.Format("{0:0.00}", txtCommission.Text));
                parameters.Add(comModel.ID1);

                conDB.AddRecordToDatabase(queryString, parameters);

                conDB.closeConnection();
   
                MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                conDB.writeLogFile("UPDATED COMMISSION RECORD: RECORD ID: " + comModel.ID1);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FAILED TO UPDATE RECORD! " + "," + ex.Message);
            }

        }

        private void getPromoName()
        {
           
            ServiceTypeModel promoServ = new ServiceTypeModel();

            string queryString = "SELECT ID, promoname, price FROM dbspa.tblpromo WHERE dbspa.tblpromo.commission = 0 AND isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while(reader.Read())
            {
                promoServ.ID1 = reader["ID"].ToString();
                promoServ.ServiceType = reader["promoname"].ToString();
                promoServ.Price = reader["price"].ToString();
                promoServ.ifPromo = true;
                cmbServiceType.Items.Add(promoServ);
                promoServ = new ServiceTypeModel();
            }
            conDB.closeConnection();

           
        }

        private void insertServiceTypeWithCommission()
        {
            try
            {
                string queryString = "INSERT INTO dbspa.tblcommissions (serviceTypeID,commission,isDeleted)VALUES(?, ?, ?)";
                List<string> parameters = new List<string>();
                ServiceTypeModel stm = cmbServiceType.SelectedItem as ServiceTypeModel;

                parameters.Add(stm.ID1);
                parameters.Add(String.Format("{0:0.00}", txtCommission.Text));
                parameters.Add(0.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                conDB.writeLogFile("ADDED COMMISSION RECORD: SERVICE TYPE ID: " + stm.ID1 + " COMMISSION: " + String.Format("{0:0.00}", txtCommission.Text));
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                getDatagridDetails();
            }
        }

        private void updatePromoNameWithCommission(string id)
        {
            string queryString = "UPDATE dbspa.tblpromo SET commission = ? WHERE ID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(txtCommission.Text);
            parameters.Add(id.ToString());

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCommission.Text))
            {
                MessageBox.Show("Please input commission value");
            }
            else if (cmbServiceType.SelectedItem == null)
            {
                MessageBox.Show("Please select Service Type value");
            }
            else
            {
                ServiceTypeModel s = cmbServiceType.SelectedItem as ServiceTypeModel;
                if(s != null)
                {
                    if (s.ifPromo)
                    {
                        updatePromoNameWithCommission(s.ID1);
                    }else
                    {
                        insertServiceTypeWithCommission();

                    }
                }
               
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCommission.Text))
            {
                MessageBox.Show("Please input commission value");
            }
            else if (cmbServiceType.SelectedItem == null)
            {
                MessageBox.Show("Please select Service Type value");
            }
            else
            {
                updateCommissionDetails(comModel);
                getDatagridDetails();
            }
        }

        private void txtCommission_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

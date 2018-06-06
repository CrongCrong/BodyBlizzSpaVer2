using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for PromoServiceDetails.xaml
    /// </summary>
    public partial class PromoServiceDetails : MetroWindow
    {
        public PromoServiceDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        PromoWindow promoWindow;
        PromoServicesModel promoServiceModel;
        string insertedPromoID;
        string queryString = "";
        List<string> parameters;

        public PromoServiceDetails(PromoWindow psw)
        {
            promoWindow = psw;
            InitializeComponent();
        }

        public PromoServiceDetails(PromoWindow psw, PromoServicesModel psm)
        {
            promoWindow = psw;
            promoServiceModel = psm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(promoServiceModel != null)
            {
                label.Content = "Promo Name: " + promoServiceModel.PromoName;
                cmbPromos.Visibility = Visibility.Hidden;
                insertedPromoID = promoServiceModel.PromoID;
                loadDataGridServices(promoServiceModel.PromoID);
            }
            
            fillListBoxServiceType(cmbServices);
            fillPromoName();
        }

        private void loadDataGridServices(string promoID)
        {
            List<PromoServicesModel> lstPromoServ = new List<PromoServicesModel>();
            PromoServicesModel promoServ = new PromoServicesModel();
            queryString = "SELECT dbspa.tblpromoservices.ID, dbspa.tblpromo.promoname, dbspa.tblservicetype.serviceType, " +
                "dbspa.tblservicetype.description FROM ((dbspa.tblpromoservices INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservices.promoID = dbspa.tblpromo.ID) " +
                "INNER JOIN dbspa.tblservicetype ON dbspa.tblpromoservices.serviceID = dbspa.tblservicetype.ID) " +
                "WHERE dbspa.tblpromoservices.isDeleted = 0 AND dbspa.tblpromoservices.promoID = ?";
            parameters = new List<string>();
            parameters.Add(promoID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while(reader.Read())
            {
                promoServ.ID = reader["ID"].ToString();
                promoServ.PromoName = reader["promoname"].ToString();
                promoServ.ServiceName = reader["description"].ToString();

                lstPromoServ.Add(promoServ);
                promoServ = new PromoServicesModel();
            }

            conDB.closeConnection();
            dgvPromoServices.ItemsSource = lstPromoServ;
        }

        private void loadDataGridDetailsForServicesInPromo()
        {
            List<PromoServicesModel> lstPromoServices = new List<PromoServicesModel>();
            PromoServicesModel promoservice = new PromoServicesModel();

            queryString = "SELECT dbspa.tblpromoservices.ID, dbspa.tblpromo.promoname, dbspa.tblservicetype.description, promoID FROM ((dbspa.tblpromoservices " +
                "INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservices.promoID = dbspa.tblpromo.ID) INNER JOIN dbspa.tblservicetype " +
                "ON dbspa.tblpromoservices.serviceID = dbspa.tblservicetype.ID) WHERE dbspa.tblpromoservices.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                promoservice.ID = reader["ID"].ToString();
                promoservice.PromoName = reader["promoname"].ToString();
                promoservice.ServiceName = reader["description"].ToString();
                promoservice.PromoID = reader["promoID"].ToString();
                lstPromoServices.Add(promoservice);
                promoservice = new PromoServicesModel();
            }
            conDB.closeConnection();

            promoWindow.dgvPromoServices.ItemsSource = lstPromoServices;

        }

        private void loadDataGridServices()
        {
            PromoModel pm = cmbPromos.SelectedItem as PromoModel;

            List<PromoServicesModel> lstPromoServ = new List<PromoServicesModel>();
            PromoServicesModel promoServ = new PromoServicesModel();
            queryString = "SELECT dbspa.tblpromoservices.ID, dbspa.tblpromo.promoname, dbspa.tblservicetype.serviceType, " +
                "dbspa.tblservicetype.description FROM((dbspa.tblpromoservices INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservices.promoID = dbspa.tblpromo.ID) " +
                "INNER JOIN dbspa.tblservicetype ON dbspa.tblpromoservices.serviceID = dbspa.tblservicetype.ID) " +
                "WHERE dbspa.tblpromoservices.isDeleted = 0 AND dbspa.tblpromoservices.promoID = ?";

            parameters = new List<string>();

            if(pm != null)
            {
                parameters.Add(pm.ID);
            }else
            {
                parameters.Add("");
            }
            
            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                promoServ.ID = reader["ID"].ToString();
                promoServ.PromoName = reader["promoname"].ToString();
                promoServ.ServiceName = reader["description"].ToString();
                
                lstPromoServ.Add(promoServ);
                promoServ = new PromoServicesModel();
            }

            conDB.closeConnection();
            dgvPromoServices.ItemsSource = lstPromoServ;
        }

        private void fillPromoName()
        {
            PromoModel promo = new PromoModel();
            queryString = "SELECT dbspa.tblpromo.ID, promoname FROM dbspa.tblpromo WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while(reader.Read())
            {
                promo.ID = reader["ID"].ToString();
                promo.PromoName = reader["promoname"].ToString();           
                cmbPromos.Items.Add(promo);
                promo = new PromoModel();
            }
            conDB.closeConnection();
        }

        private void fillListBoxServiceType(ComboBox combo)
        {
            try
            {

                queryString = "SELECT ID, serviceType AS 'SERVICE TYPE', price AS 'PRICE', description FROM dbspa.tblservicetype WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    ServiceTypeModel stm = new ServiceTypeModel();
                    stm.ServiceType = reader["SERVICE TYPE"].ToString();
                    stm.Price = reader["price"].ToString();
                    stm.ID1 = reader["ID"].ToString();
                    stm.Description = reader["description"].ToString();

                    combo.Items.Add(stm);
                    
                }
                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void addServiceToPromo(string ID)
        {
            queryString = "INSERT INTO dbspa.tblpromoservices (promoID, serviceID, isDeleted) VALUES (?,?,?)";
            parameters = new List<string>();
            parameters.Add(ID);
            parameters.Add(cmbServices.SelectedValue.ToString());
            parameters.Add("0");

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            insertedPromoID = ID;
        }

        private bool verifySelection()
        {
            bool ifAllCorrect = false;
            PromoModel selectPromo = cmbPromos.SelectedItem as PromoModel;
            ServiceTypeModel selectService = cmbServices.SelectedItem as ServiceTypeModel;

            if(selectPromo == null)
            {
                MessageBox.Show("Please select Promo Name!");
            }else if(selectService == null)
            {
                MessageBox.Show("Please select Service!");
            }else
            {
                ifAllCorrect = true;
            }           
            return ifAllCorrect;
        }

        private void deleteRecord()
        {
            queryString = "UPDATE dbspa.tblpromoservices SET isDeleted = 1 WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(promoServiceModel.ID);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteRecord();
            MessageBox.Show("SERVICE DELETED SUCCESSFULLY!");
            loadDataGridServices(promoServiceModel.PromoID);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(cmbPromos.Visibility != Visibility.Hidden)
            {
                if (verifySelection())
                {
                    PromoModel promo = cmbPromos.SelectedItem as PromoModel;
                    addServiceToPromo(promo.ID);
                    //label.Content = "Promo Name: " + promo.PromoName;
                    loadDataGridServices(promo.ID);
                }
            }else
            {
                ServiceTypeModel selectService = cmbServices.SelectedItem as ServiceTypeModel;
                if(selectService != null)
                {
                    addServiceToPromo(insertedPromoID);
                    loadDataGridServices(insertedPromoID);
                }else
                {
                    MessageBox.Show("Please select Service!");
                }
            }
            
        }

        private void cmbPromos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadDataGridServices();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loadDataGridDetailsForServicesInPromo();
        }
    }
}

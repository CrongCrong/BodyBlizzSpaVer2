using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for PromoWindow.xaml
    /// </summary>
    public partial class PromoWindow : MetroWindow
    {
        public PromoWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        string queryString = "";
        List<string> parameters;


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadDataGridDetails();
            loadDataGridDetailsForServicesInPromo();
        }

        private void loadDataGridDetails()
        {
            PromoModel promo = new PromoModel();
            List<PromoModel> lstPromos = new List<PromoModel>();

            queryString = "SELECT ID, promoname, price, commission FROM dbspa.tblpromo WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                promo.ID = reader["ID"].ToString();
                promo.PromoName = reader["promoname"].ToString();
                promo.PromoPrice = Convert.ToDouble(reader["price"].ToString());
                promo.Commission = reader["commission"].ToString();
                lstPromos.Add(promo);
                promo = new PromoModel();
            }
            conDB.closeConnection();
            dgvPromos.ItemsSource = lstPromos;
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

            dgvPromoServices.ItemsSource = lstPromoServices;

        }

        private List<PromoServicesModel> loadServices(string promoID)
        {
            List<PromoServicesModel> lstPromoServices = new List<PromoServicesModel>();
            PromoServicesModel promoService = new PromoServicesModel();

            queryString = "SELECT dbspa.tblpromoservices.ID, dbspa.tblpromo.promoname, dbspa.tblservicetype.serviceType " +
                "FROM((dbspa.tblpromoservices INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservices.promoID = dbspa.tblpromo.ID) " +
                "INNER JOIN dbspa.tblservicetype ON dbspa.tblpromoservices.serviceID = dbspa.tblservicetype.ID) " +
                "WHERE dbspa.tblpromoservices.isDeleted = 0 AND dbspa.tblpromoservices.promoID = ?";
            parameters = new List<string>();
            parameters.Add(promoID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                promoService.ID = reader["ID"].ToString();
                promoService.PromoName = reader["promoname"].ToString();
                promoService.ServiceName = reader["serviceType"].ToString();

                lstPromoServices.Add(promoService);
                promoService = new PromoServicesModel();
            }

            conDB.closeConnection();

            return lstPromoServices;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PromoDetails promoDetails = new PromoDetails(this);
            promoDetails.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            PromoModel proMod = dgvPromos.SelectedItem as PromoModel;
            if(proMod != null)
            {
                PromoDetails promoDetails = new PromoDetails(this, proMod);
                promoDetails.ShowDialog();
            }else
            {
                MessageBox.Show("No Records selected!");
            }
        }

        private void btnAddServiceToPromo_Click(object sender, RoutedEventArgs e)
        {
            PromoServiceDetails promoServ = new PromoServiceDetails(this);
            promoServ.ShowDialog();
        }

        private void btnEditServiceToPromo_Click(object sender, RoutedEventArgs e)
        {
            PromoServicesModel promoServModel = dgvPromoServices.SelectedItem as PromoServicesModel;

            if (promoServModel != null)
            {
                PromoServiceDetails promoServ = new PromoServiceDetails(this, promoServModel);

                //promoServ.dgvPromoServices.ItemsSource = loadServices(promoServModel.PromoID);

                promoServ.ShowDialog();
            }
        }
    }
}

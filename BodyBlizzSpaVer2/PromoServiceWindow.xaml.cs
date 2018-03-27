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
    /// Interaction logic for PromoServiceWindow.xaml
    /// </summary>
    public partial class PromoServiceWindow : MetroWindow
    {
        public PromoServiceWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadDataGridDetails();
        }

        private void loadDataGridDetails()
        {
            List<PromoServicesModel> lstPromoServices = new List<PromoServicesModel>();
            PromoServicesModel promoservice = new PromoServicesModel();

            string queryString = "SELECT dbspa.tblpromoservices.ID, dbspa.tblpromo.ID as 'PROMO ID', dbspa.tblpromo.promoname, dbspa.tblpromo.price " +
                "FROM(dbspa.tblpromo INNER JOIN dbspa.tblpromoservices ON dbspa.tblpromo.ID = dbspa.tblpromoservices.promoID) " +
                "WHERE dbspa.tblpromo.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while(reader.Read())
            {
                promoservice.ID = reader["ID"].ToString();
                promoservice.PromoID = reader["PROMO ID"].ToString();
                promoservice.PromoName = reader["promoname"].ToString();
                promoservice.PromoPrice = reader["price"].ToString();
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

            string queryString = "SELECT dbspa.tblpromoservices.ID, dbspa.tblpromo.promoname, dbspa.tblservicetype.serviceType " +
                "FROM((dbspa.tblpromoservices INNER JOIN dbspa.tblpromo ON dbspa.tblpromoservices.promoID = dbspa.tblpromo.ID) " +
                "INNER JOIN dbspa.tblservicetype ON dbspa.tblpromoservices.serviceID = dbspa.tblservicetype.ID) " +
                "WHERE dbspa.tblpromoservices.isDeleted = 0 AND dbspa.tblpromoservices.promoID = ?";
            List<string> parameters = new List<string>();
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
            PromoServiceDetails promoServ = new PromoServiceDetails();
            promoServ.ShowDialog();
            
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            PromoServicesModel promoServModel = dgvPromoServices.SelectedItem as PromoServicesModel;

            if(promoServModel != null)
            {
                PromoServiceDetails promoServ = new PromoServiceDetails();

                promoServ.dgvPromoServices.ItemsSource = loadServices(promoServModel.PromoID);

                promoServ.ShowDialog();
            }

        }


    }
}

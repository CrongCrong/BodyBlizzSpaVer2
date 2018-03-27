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
    /// Interaction logic for PromoDetails.xaml
    /// </summary>
    public partial class PromoDetails : MetroWindow
    {
        public PromoDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        PromoWindow promoWindow;
        PromoModel promoModel;

        public PromoDetails(PromoWindow pw)
        {
            promoWindow = pw;
            InitializeComponent();
        }

        public PromoDetails(PromoWindow pw, PromoModel pm)
        {
            promoWindow = pw;
            promoModel = pm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if(promoModel != null)
            {
                txtPromoName.Text = promoModel.PromoName;
                txtPromoPrice.Text = promoModel.PromoPrice.ToString();
                txtCommission.Text = promoModel.Commission;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Hidden;
            }
        }

        private void loadDataGridDetails()
        {
            PromoModel promo = new PromoModel();
            List<PromoModel> lstPromos = new List<PromoModel>();

            string queryString = "SELECT ID, promoname, price, commission FROM dbspa.tblpromo WHERE isDeleted = 0";

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
            promoWindow.dgvPromos.ItemsSource = lstPromos;
        }

        private void savePromoRecord()
        {
            string queryString = "INSERT INTO dbspa.tblpromo (promoname, price, commission, isDeleted) VALUES (?,?,?,?)";
            List<string> parameters = new List<string>();

            parameters.Add(txtPromoName.Text);
            parameters.Add(String.Format("{0:0.00}", txtPromoPrice.Text));
            parameters.Add(txtCommission.Text);
            parameters.Add("0");

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void updatePromoRecord()
        {
            string queryString = "UPDATE dbspa.tblpromo SET promoname = ?, price = ?, commission = ? WHERE ID = ?";
            List<string> parameters = new List<string>();
            parameters.Add(txtPromoName.Text);
            parameters.Add(String.Format("{0:0.00}", txtPromoPrice.Text));
            parameters.Add(txtCommission.Text);
            parameters.Add(promoModel.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(txtPromoName.Text))
            {
                MessageBox.Show("Please input Promo name!");
            }
            else if (string.IsNullOrEmpty(txtPromoPrice.Text))
            {
                MessageBox.Show("Please input Promo price!");
            }
            else if (string.IsNullOrEmpty(txtCommission.Text))
            {
                MessageBox.Show("Please input commission!");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(checkFields())
            {
                savePromoRecord();
                loadDataGridDetails();
                MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                updatePromoRecord();
                loadDataGridDetails();
                MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtPromoPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
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

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
    /// Interaction logic for DiscountDetails.xaml
    /// </summary>
    public partial class DiscountDetails : MetroWindow
    {
        public DiscountDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();

        DiscountWindow discountForm;
        DiscountModel discount;

        public DiscountDetails(DiscountWindow df)
        {
            discountForm = df;
            InitializeComponent();
        }

        public DiscountDetails(DiscountWindow df, DiscountModel dm)
        {
            discountForm = df;
            discount = dm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if (discount != null)
            {
                txtDescription.Text = discount.Description;
                txtDiscount.Text = discount.Discount;
                btnUpdate.Visibility = Visibility.Visible;
                btnAdd.Visibility = Visibility.Hidden;
            }
        }

        private bool checkFields()
        {
            bool ifCorrect = false;
            try
            {
                if (string.IsNullOrEmpty(txtDiscount.Text))
                {
                    MessageBox.Show("Please input Discount value!");
                }
                else if (string.IsNullOrEmpty(txtDescription.Text))
                {
                    MessageBox.Show("Please input Description value!");
                }
                else
                {
                    ifCorrect = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return ifCorrect;
        }

        private void loadDataGridDetails()
        {
            List<DiscountModel> lstDiscounts = new List<DiscountModel>();
            DiscountModel ds = new DiscountModel();

            string queryString = "SELECT ID, discount as 'DISCOUNT (%)', description AS 'DESCRIPTION' FROM dbspa.tbldiscount WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while(reader.Read())
            {
                ds.ID1 = reader["ID"].ToString();
                ds.Discount = reader["DISCOUNT (%)"].ToString();
                ds.Description = reader["DESCRIPTION"].ToString();
                lstDiscounts.Add(ds);
                ds = new DiscountModel();
            }

            discountForm.dgvDiscounts.ItemsSource = lstDiscounts;
            conDB.closeConnection();

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                try
                {
                    string queryString = "INSERT INTO dbspa.tbldiscount (discount, description,isDeleted)" +
                         "VALUES(?,?,?)";
                    List<string> parameters = new List<string>();

                    parameters.Add(txtDiscount.Text);
                    parameters.Add(String.Format("{0:0.00}", txtDescription.Text));
                    parameters.Add(0.ToString());
                    conDB.AddRecordToDatabase(queryString, parameters);
                    conDB.closeConnection();

                    loadDataGridDetails();
                    MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                try
                {
                    string queryString = "UPDATE dbspa.tbldiscount SET discount = ?, description = ? " +
                        "WHERE ID = ?";
                    List<string> parameters = new List<string>();
                    parameters.Add(String.Format("{0:0.00}", txtDiscount.Text));
                    parameters.Add(txtDescription.Text);
                    parameters.Add(discount.ID1);

                    conDB.AddRecordToDatabase(queryString, parameters);
                    conDB.closeConnection();
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

        private void txtDiscount_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

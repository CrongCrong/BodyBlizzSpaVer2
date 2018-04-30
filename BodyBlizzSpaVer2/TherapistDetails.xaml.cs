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
    /// Interaction logic for TherapistDetails.xaml
    /// </summary>
    public partial class TherapistDetails : MetroWindow
    {
        public TherapistDetails()
        {
            InitializeComponent();
        }

        public TherapistDetails(Therapist t)
        {
            therapist = t;
            InitializeComponent();
        }

        public TherapistDetails(Therapist t, TherapistModel tm)
        {
            tModel = tm;
            therapist = t;
            InitializeComponent();
        }

        public Therapist therapist;
        public TherapistModel tModel;
        ConnectionDB conDB = new ConnectionDB();

       
        public List<TherapistModel> getDatagridDetails()
        {
            List<TherapistModel> lstTherapist = new List<TherapistModel>();
            TherapistModel t = new TherapistModel();
            try
            {
                string queryString = "SELECT ID, firstName as 'FIRST NAME', lastName as 'LAST NAME', wage as 'WAGE' FROM dbspa.tbltherapist" +
                    " WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    t.ID1 = reader["ID"].ToString();
                    t.FirstName = reader["FIRST NAME"].ToString();
                    t.LastName = reader["LAST NAME"].ToString();
                    t.Wage = reader["WAGE"].ToString();
                    lstTherapist.Add(t);
                    t = new TherapistModel();
                }

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return lstTherapist;
        }

        private bool checkFields()
        {
            bool ifCorrect = false;
            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                MessageBox.Show("Please input First Name!");
            }
            else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Please input Last Name!");
            }
            else if (string.IsNullOrEmpty(txtWage.Text))
            {
                MessageBox.Show("Please input Wage value!");
            }
            else
            {
                ifCorrect = true;
            }

            return ifCorrect;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkFields())
                {
                    string queryString = "INSERT INTO dbspa.tbltherapist (firstName,lastName,wage,description,isDeleted)" +
                        "VALUES(?,?,?,?,?)";
                    List<string> parameters = new List<string>();
                    string fullName = txtFirstName.Text + " " + txtLastName.Text;
                    parameters.Add(txtFirstName.Text);
                    parameters.Add(txtLastName.Text);
                    parameters.Add(txtWage.Text);
                    parameters.Add(fullName);
                    parameters.Add(0.ToString());

                    conDB.AddRecordToDatabase(queryString, parameters);
                    conDB.closeConnection();

                    therapist.dgvTherapist.ItemsSource = getDatagridDetails();

                    MessageBox.Show("RECORD SAVED SUCCESSFULLY!");

                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkFields())
                {
                    try
                    {
                        string queryString = "UPDATE dbspa.tbltherapist SET firstName = ?, lastName = ?, wage = ?, description = ? WHERE ID = ?";
                        List<string> parameters = new List<string>();
                        parameters.Add(txtFirstName.Text);
                        parameters.Add(txtLastName.Text);
                        parameters.Add(txtWage.Text);
                        parameters.Add(txtFirstName.Text + " " + txtLastName.Text);
                        parameters.Add(tModel.ID1);

                        conDB.AddRecordToDatabase(queryString, parameters);

                        therapist.dgvTherapist.ItemsSource = getDatagridDetails();

                        MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            if (tModel != null)
            {
                if (tModel.IfEditDetails)
                {
                    txtFirstName.Text = tModel.FirstName;
                    txtLastName.Text = tModel.LastName;
                    txtWage.Text = tModel.Wage;
                    btnUpdate.Visibility = Visibility.Visible;
                    btnSave.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtWage_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

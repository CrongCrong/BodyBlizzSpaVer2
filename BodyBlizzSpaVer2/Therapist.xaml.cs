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
    /// Interaction logic for Therapist.xaml
    /// </summary>
    public partial class Therapist : MetroWindow
    {
        public Therapist()
        {
            InitializeComponent();
        }


        public TherapistModel therapist = new TherapistModel();
        ConnectionDB conDB = new ConnectionDB();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgvTherapist.ItemsSource = getDatagridDetails();
        }

        public List<TherapistModel> getDatagridDetails()
        {
            List<TherapistModel> lstTherapist = new List<TherapistModel>();
            TherapistModel t = new TherapistModel();
            try
            {
                string queryString = "SELECT ID, firstName as 'FIRST NAME', lastName as 'LAST NAME', wage as 'WAGE' FROM dbspa.tbltherapist" +
                    " WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while(reader.Read())
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

        private void deleteTherapistRecord(int id)
        {

            try
            {
                string queryString = "UPDATE dbspa.tbltherapist SET isDeleted = ? WHERE ID = ?";
                List<string> parameters = new List<string>();
                parameters.Add(1.ToString());
                parameters.Add(id.ToString());

                conDB.AddRecordToDatabase(queryString, parameters);
                conDB.closeConnection();

                dgvTherapist.ItemsSource = getDatagridDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            TherapistDetails td = new TherapistDetails(this);
            td.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            TherapistModel tm = dgvTherapist.SelectedItem as TherapistModel;
            if(tm != null)
            {
                tm.IfEditDetails = true;
                TherapistDetails cd = new TherapistDetails(this, tm);
                cd.ShowDialog();
            }else
            {
                MessageBox.Show("No record selected!");
            }           
        }

        private void btnCompute_Click(object sender, RoutedEventArgs e)
        {
            TherapistModel tm = dgvTherapist.SelectedItem as TherapistModel;

            if(tm != null)
            {
                TherapistSalary ts = new TherapistSalary(tm);
                ts.ShowDialog();
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
                TherapistModel cm = dgvTherapist.SelectedItem as TherapistModel;

                if (cm != null)
                {
                    int id = Convert.ToInt32(cm.ID1);

                    if (id != 0)
                    {
                        deleteTherapistRecord(id);
                        System.Windows.MessageBox.Show("Record deleted successfuly!");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No record selected!");
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCashAdvance_Click(object sender, RoutedEventArgs e)
        {
            CashAdvanceWindow caw = new CashAdvanceWindow();
            caw.ShowDialog();
        }

        private void btnLoans_Click(object sender, RoutedEventArgs e)
        {
            LoansWindow loansWindow = new LoansWindow();
            loansWindow.ShowDialog();
        }
    }
}

using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for CashAdvanceWindow.xaml
    /// </summary>
    public partial class CashAdvanceWindow : MetroWindow
    {
        public CashAdvanceWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        string queryString = "";
        List<string> parameters;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgvCashAdvance.ItemsSource = loadDataGridDetails();
            loadTherapist();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                saveCashAdvanceRecord();
                System.Windows.Forms.MessageBox.Show("RECORD ADDED SUCCESSFULLY!");
                
                dgvCashAdvance.ItemsSource = loadDataGridDetails();
                clearFields();
            }
        }

        private void loadTherapist()
        {
            try
            {
                queryString = "SELECT ID, firstName, lastName, wage, description FROM dbspa.tbltherapist WHERE (isDeleted = 0)";

                MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

                while (reader.Read())
                {
                    TherapistModel tm = new TherapistModel();
                    tm.ID1 = reader["ID"].ToString();
                    tm.FirstName = reader["firstName"].ToString();
                    tm.LastName = reader["lastName"].ToString();
                    tm.Wage = reader["wage"].ToString();
                    tm.Description = tm.FirstName + " " + tm.LastName;
                    cmbTherapist.Items.Add(tm);
                }

                conDB.closeConnection();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(dateCashAdvance.Text))
            {
                System.Windows.Forms.MessageBox.Show("Please select date!");
            }else if(cmbTherapist.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("Please select therapist!");
            }else if (string.IsNullOrEmpty(txtCash.Text))
            {
                System.Windows.Forms.MessageBox.Show("Please input Cash value!");
            }else
            {
                ifAllCorrect = true;
            }


            return ifAllCorrect;
        }

        private void clearFields()
        {
            dateCashAdvance.Text = "";
            txtCash.Text = "";
        }

        private List<CashAdvanceModel> loadDataGridDetails()
        {
            List<CashAdvanceModel> lstCashAdvance = new List<CashAdvanceModel>();
            CashAdvanceModel cashAdvance = new CashAdvanceModel();

            queryString = "SELECT dbspa.tblcashadvance.ID, Date, therapistID, dbspa.tbltherapist.description, cash FROM " +
                "(dbspa.tblcashadvance INNER JOIN dbspa.tbltherapist ON dbspa.tblcashadvance.therapistID = dbspa.tbltherapist.ID) " +
                "WHERE dbspa.tblcashadvance.isDeleted = 0 ORDER BY dbspa.tblcashadvance.ID DESC";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                cashAdvance.ID = reader["ID"].ToString();
                DateTime dte = DateTime.Parse(reader["Date"].ToString());
                cashAdvance.Date = dte.ToShortDateString();
                cashAdvance.Therapist = reader["description"].ToString();
                cashAdvance.TherapistID = reader["therapistID"].ToString();
                cashAdvance.Cash = reader["cash"].ToString();
                lstCashAdvance.Add(cashAdvance);
                cashAdvance = new CashAdvanceModel();
            }
            return lstCashAdvance;
        }
        
        private void saveCashAdvanceRecord()
        {
            queryString = "INSERT INTO dbspa.tblcashadvance (Date, therapistID, cash, isDeleted) VALUES (?,?,?,0)";

            parameters = new List<string>();
            DateTime dte = DateTime.Parse(dateCashAdvance.Text);
            parameters.Add(dte.Year + "-" + dte.Month + "-" + dte.Day);
            parameters.Add(cmbTherapist.SelectedValue.ToString());
            parameters.Add(txtCash.Text);

            conDB.AddRecordToDatabase(queryString, parameters);

            conDB.closeConnection();

            conDB.writeLogFile("ADDED CASH ADVANCE: DETAILS: DATE: " + dte.ToShortDateString() + " THERAPIS: " + cmbTherapist.SelectedValue.ToString() + 
                " CASH: " + txtCash.Text);
                
                
        }

        private void deleteCashAdvanceRecord(string recID)
        {
            queryString = "UPDATE dbspa.tblcashadvance SET isDeleted = 1 WHERE ID = ?";
            parameters = new List<string>();
            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("DELETED CASH ADVANCE: RECORD ID: " + recID);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                CashAdvanceModel t = dgvCashAdvance.SelectedItem as CashAdvanceModel;

                if (t != null)
                {
                    deleteCashAdvanceRecord(t.ID);
                    dgvCashAdvance.ItemsSource = loadDataGridDetails();
                    System.Windows.MessageBox.Show("RECORD DELETED SUCCESSFULLY!");
                    conDB.writeLogFile("DELETED CASH ADVANCE RECORD: ID: " + t.ID);
                }
            }
        }
    }
}

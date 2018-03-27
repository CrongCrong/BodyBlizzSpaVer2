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
    /// Interaction logic for LoansDetailsWindow.xaml
    /// </summary>
    public partial class LoansDetailsWindow : MetroWindow
    {
        public LoansDetailsWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        LoansWindow loansWindow;

        public LoansDetailsWindow(LoansWindow lw)
        {
            loansWindow = lw;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadTherapist();
        }

        private void loadTherapist()
        {
            try
            {
                string queryString = "SELECT ID, firstName, lastName, wage, description FROM dbspa.tbltherapist WHERE (isDeleted = 0)";

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

        private void saveLoanDetails()
        {

            string queryString = "INSERT INTO dbspa.tblloans (loandate, therapistID, loanamount, isDeleted) VALUES (?,?,?,0)";
            List<string> parameters = new List<string>();
            DateTime dte = DateTime.Parse(dateLoan.Text);
            parameters.Add(dte.Year + "-" + dte.Month + "-" + dte.Day);
            parameters.Add(cmbTherapist.SelectedValue.ToString());
            parameters.Add(txtLoanAmount.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

            conDB.writeLogFile("ADDED LOAN TO THERAPIST: DETAILS: DATE -" + dte.ToShortDateString() + " THERAPIST ID:" + cmbTherapist.SelectedValue.ToString() +
                " LOAN AMOUNT: " + txtLoanAmount.Text);

        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(dateLoan.Text))
            {
                MessageBox.Show("Please select date!");
            }else if(cmbTherapist.SelectedItem == null)
            {
                MessageBox.Show("Please select therapist!");
            }else if (string.IsNullOrEmpty(txtLoanAmount.Text))
            {
                MessageBox.Show("Please input loan amount!");
            }else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private List<LoanModel> loadDataGridDetails()
        {
            List<LoanModel> lstLoanModel = new List<LoanModel>();
            LoanModel loanModel = new LoanModel();

            string queryString = "SELECT dbspa.tblloans.ID, therapistID, dbspa.tbltherapist.description, loanamount, loandate FROM (dbspa.tblloans INNER JOIN " +
                "dbspa.tbltherapist ON dbspa.tblloans.therapistID = dbspa.tbltherapist.ID) WHERE dbspa.tblloans.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                loanModel.ID = reader["ID"].ToString();
                loanModel.TherapistID = reader["therapistID"].ToString();
                loanModel.Therapist = reader["description"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                loanModel.LoanDate = dte.ToShortDateString();
                loanModel.LoanAmount = reader["loanamount"].ToString();

                lstLoanModel.Add(loanModel);
                loanModel = new LoanModel();
            }

            conDB.closeConnection();

            return lstLoanModel;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                saveLoanDetails();
                MessageBox.Show("RECORD SUCCESSFULLY SAVED!");
                loansWindow.dgvLoans.ItemsSource = loadDataGridDetails();
                this.Close();
            }
        }
    }
}

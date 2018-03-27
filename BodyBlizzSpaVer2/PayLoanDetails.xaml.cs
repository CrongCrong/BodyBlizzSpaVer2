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
    /// Interaction logic for PayLoanDetails.xaml
    /// </summary>
    public partial class PayLoanDetails : MetroWindow
    {
        public PayLoanDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        LoanBalanceWindow loanBalWindow;
        LoanModel loanModel;

        public PayLoanDetails(LoanBalanceWindow lbw, LoanModel lm)
        {
            loanBalWindow = lbw;
            loanModel = lm;
            InitializeComponent();
        }

        

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (checkFields())
            {
                savePayRecord();
                MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                loanBalWindow.dgvLoansToPay.ItemsSource = getLoansForTherapist();
                this.Close();
            }
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(datePaidPicker.Text))
            {
                MessageBox.Show("Please select date!");
            }else if (string.IsNullOrEmpty(txtAmtPay.Text))
            {
                MessageBox.Show("Please input value for Amt. to pay!");
            }else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void savePayRecord()
        {

            string queryString = "INSERT INTO dbspa.tblloanbalance (therapistID, datepaid, dateloan, loanID, amount, isDeleted) " +
                "VALUES(?,?,?,?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(loanModel.TherapistID);
            DateTime dte = DateTime.Parse(datePaidPicker.Text);
            parameters.Add(dte.Year + "-" + dte.Month + "-" + dte.Day);
            dte = DateTime.Parse(loanModel.LoanDate);
            parameters.Add(dte.Year + "-" + dte.Month + "-" + dte.Day);
            parameters.Add(loanModel.RecordID);
            parameters.Add(txtAmtPay.Text);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

            conDB.writeLogFile("PAID AMOUNT TO LOAN ID: " + loanModel.RecordID + " AMOUNT OF: " + txtAmtPay.Text + " LOAN DATE: " + dte.ToShortDateString() +
                " THERAPIST: " + loanModel.TherapistID);
        }

        private List<LoanModel> getLoansForTherapist()
        {
            List<LoanModel> lstLoanBalance = new List<LoanModel>();
            LoanModel loanMod = new LoanModel();

            string queryString = "SELECT tbl2.ID, tbl2.loandate, tbl2.therapistID, tbl2.loanamount, ifPaid, " +
                "tbl2.loanamount - (SELECT SUM(dbspa.tblloanbalance.amount) as balance FROM dbspa.tblloanbalance WHERE isDeleted = 0 AND loanID = tbl2.ID)" +
                " as balance FROM dbspa.tblloans as tbl2 WHERE tbl2.isDeleted = 0 AND tbl2.therapistID = ?";

            List<string> parameters = new List<string>();
            parameters.Add(loanModel.TherapistID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                loanMod.RecordID = reader["ID"].ToString();
                DateTime dte = DateTime.Parse(reader["loandate"].ToString());
                loanMod.LoanDate = dte.ToShortDateString();
                loanMod.TherapistID = reader["therapistID"].ToString();
                loanMod.LoanAmount = reader["loanamount"].ToString();
                string strBal = reader["balance"].ToString();

                string strPaid = reader["ifPaid"].ToString();

                if (strPaid.Equals("1"))
                {
                    loanMod.ifPaid = true;
                }
                else
                {
                    loanMod.ifPaid = false;
                }

                if (string.IsNullOrEmpty(strBal))
                {
                    loanMod.LoanBalance = loanMod.LoanAmount;
                }
                else
                {
                    loanMod.LoanBalance = strBal;
                }
                lstLoanBalance.Add(loanMod);
                loanMod = new LoanModel();
            }

            conDB.closeConnection();

            return lstLoanBalance;
        }


    }
}

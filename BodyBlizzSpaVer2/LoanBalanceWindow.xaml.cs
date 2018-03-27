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
    /// Interaction logic for LoanBalanceWindow.xaml
    /// </summary>
    public partial class LoanBalanceWindow : MetroWindow
    {
        public LoanBalanceWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        LoansWindow loansWindow;
        LoanModel loanModel;

        public LoanBalanceWindow(LoansWindow lw, LoanModel lm)
        {
            loansWindow = lw;
            loanModel = lm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            if(loanModel != null)
            {
                lblTherapistName.Content = "Loans for " + loanModel.Therapist;
                dgvLoansToPay.ItemsSource = getLoansForTherapist();
            }
        }

        private List<string> getLoanHistory(string strRecordID, string therapistID)
        {
            List<string> lstString = new List<string>();
            string str = "";
            string queryString = "SELECT therapistID, datepaid, amount FROM dbspa.tblloanbalance WHERE loanID = ? AND therapistID = ? AND isDeleted = 0";

            List<string> parameters = new List<string>();
            parameters.Add(strRecordID);
            parameters.Add(therapistID); 

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                DateTime dte = DateTime.Parse(reader["datepaid"].ToString());
                str = dte.ToShortDateString() + " - " + reader["amount"].ToString();
                lstString.Add(str);
                str = "";
                    
            }
            conDB.closeConnection();

            return lstString;
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
                }else
                {
                    loanMod.ifPaid = false;
                }

                if (string.IsNullOrEmpty(strBal))
                {
                    loanMod.LoanBalance = loanMod.LoanAmount;
                }else
                {
                    loanMod.LoanBalance = strBal;
                }
                lstLoanBalance.Add(loanMod);
                loanMod = new LoanModel();
            }

            conDB.closeConnection();

            return lstLoanBalance;
        }

        private void updateLoansRecord(string strPaid, string strRecordID)
        {
            string queryString = "UPDATE dbspa.tblloans SET ifPaid = ? WHERE ID =?";
            List<string> parameters = new List<string>();
            parameters.Add(strPaid);
            parameters.Add(strRecordID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();
            conDB.writeLogFile("LOANS SET TO FULLY PAID: RECORD ID: " + strRecordID);

        }

        private void checkUpdateIfSuccessful()
        {
              
            for (int i = 0; i < dgvLoansToPay.Items.Count -1; i++)
            {
                // progressBar.IsActive = true;
                var item = dgvLoansToPay.Items[i];
                var mycheckbox = dgvLoansToPay.Columns[3].GetCellContent(item) as CheckBox;
                LoanModel smms = item as LoanModel;
                if (smms != null)
                {
                    if ((bool)mycheckbox.IsChecked)
                    {
                        if (Convert.ToDouble(smms.LoanBalance) == 0)
                        {
                            updateLoansRecord("1", smms.RecordID);
                        }
                        else
                        {
                            MessageBox.Show("Loan: " + smms.LoanAmount + " still have remaining balance!.");
                        }
                    }else
                    {
                        updateLoansRecord("0", smms.RecordID);
                    }                   
                }
            }
           
            
        }

        private List<LoanModel> loadDataGridLoanBalance()
        {
            List<LoanModel> lstloanBalance = new List<LoanModel>();
            LoanModel loanBalance = new LoanModel();

            string queryString = "SELECT dbspa.tblloans.ID, therapistID, dbspa.tbltherapist.description, Count(*) as cnt " +
                "FROM(dbspa.tblloans INNER JOIN dbspa.tbltherapist ON dbspa.tblloans.therapistID = dbspa.tbltherapist.ID) " +
                "WHERE dbspa.tblloans.isDeleted = 0 GROUP BY therapistID";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                loanBalance.TherapistID = reader["therapistID"].ToString();
                loanBalance.Therapist = reader["description"].ToString();
                loanBalance.TotalLoans = reader["cnt"].ToString();
                lstloanBalance.Add(loanBalance);
                loanBalance = new LoanModel();
            }

            conDB.closeConnection();

            return lstloanBalance;
        }

        private void btnPayLoan_Click(object sender, RoutedEventArgs e)
        {
            LoanModel loanMod = dgvLoansToPay.SelectedItem as LoanModel;

            if(loanMod != null)
            {
                if(Convert.ToDouble(loanMod.LoanBalance) > 0)
                {
                    PayLoanDetails pld = new PayLoanDetails(this, loanMod);
                    pld.ShowDialog();
                }else
                {
                    MessageBox.Show("SELECTED LOAN HAVE 0 BALANCE!");
                }
                
            }

        }


        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoanModel lm = dgvLoansToPay.SelectedItem as LoanModel;
            List<string> lstStr = new List<string>();
            txtDetails.Text = "";
            // execute some code
            if (lm != null)
            {
                lstStr = getLoanHistory(lm.RecordID, lm.TherapistID);
            }
            
            foreach (string s in lstStr)
            {
                txtDetails.Text += s + "\n";
            }
        }

        private void btnUpdateLoan_Click(object sender, RoutedEventArgs e)
        {
            progressBar.IsActive = true;
            checkUpdateIfSuccessful();
            dgvLoansToPay.ItemsSource = getLoansForTherapist();
            loansWindow.dgvLoanBalance.ItemsSource = loadDataGridLoanBalance();
            progressBar.IsActive = false;
        }

        private void btnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            LoanModel lMod = dgvLoansToPay.SelectedItem as LoanModel;
            if(lMod != null)
            {
                LoanRecordsHistory loanHistoryView = new LoanRecordsHistory(this, lMod);
                loanHistoryView.ShowDialog();
            }else
            {
                MessageBox.Show("No record selected!");
            }
           
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }
    }
}

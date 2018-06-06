using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for LoanRecordsHistory.xaml
    /// </summary>
    public partial class LoanRecordsHistory : MetroWindow
    {
        public LoanRecordsHistory()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        LoanBalanceWindow loanWindow;
        LoanModel loanModel;
        string therapistID = "";
        string queryString = "";
        List<string> parameters;

        public LoanRecordsHistory(LoanBalanceWindow lbw, LoanModel lm)
        {
            loanWindow = lbw;
            loanModel = lm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(loanModel != null)
            {
                dgvLoanHistory.ItemsSource = loadRecordsHistory();
            }
        }

        private List<LoanModel> loadRecordsHistory()
        {
            List<LoanModel> lstLoanHistory = new List<LoanModel>();
            LoanModel lm = new LoanModel();

            queryString = "SELECT ID, therapistID, datepaid, amount FROM dbspa.tblloanbalance WHERE loanID = ? AND isDeleted = 0";
            parameters = new List<string>();
            parameters.Add(loanModel.RecordID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                therapistID = reader["therapistID"].ToString();
                lm.RecordID = reader["ID"].ToString();
                DateTime dte = DateTime.Parse(reader["datepaid"].ToString());
                lm.DatePaid = dte.ToShortDateString();
                lm.LoanAmount = reader["amount"].ToString();
                lstLoanHistory.Add(lm);
                lm = new LoanModel();
            }

            conDB.closeConnection();

            return lstLoanHistory;
        }

        private List<LoanModel> getLoansForTherapist()
        {
            List<LoanModel> lstLoanBalance = new List<LoanModel>();
            LoanModel loanMod = new LoanModel();

            queryString = "SELECT tbl2.ID, tbl2.loandate, tbl2.therapistID, tbl2.loanamount, ifPaid, " +
                "tbl2.loanamount - (SELECT SUM(dbspa.tblloanbalance.amount) as balance FROM dbspa.tblloanbalance WHERE isDeleted = 0 AND loanID = tbl2.ID)" +
                " as balance FROM dbspa.tblloans as tbl2 WHERE tbl2.isDeleted = 0 AND tbl2.therapistID = ?";

            parameters = new List<string>();
            parameters.Add(therapistID);

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

        private void deleteRecord(string recID)
        {
            queryString = "UPDATE dbspa.tblloanbalance SET isDeleted = 1 WHERE ID = ?";

            parameters = new List<string>();
            parameters.Add(recID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

            conDB.writeLogFile("DELETED PREVIOUS AMOUNT PAID TO LOAN : LOAN BALANCE RECORD:" + recID + " tblloanbalance");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure you want to Delete record?", "Delete Record", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                LoanModel loanMod = dgvLoanHistory.SelectedItem as LoanModel;

                if(loanMod != null)
                {
                    deleteRecord(loanMod.RecordID);
                    dgvLoanHistory.ItemsSource = loadRecordsHistory();
                    MessageBox.Show("RECORD DELETED SUCCESSFULLY!");
                }
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loanWindow.dgvLoansToPay.ItemsSource = getLoansForTherapist();
        }
    }
}

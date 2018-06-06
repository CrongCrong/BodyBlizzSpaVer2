using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for LoansWindow.xaml
    /// </summary>
    public partial class LoansWindow : MetroWindow
    {
        public LoansWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        string queryString = "";
        List<string> parameters;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgvLoans.ItemsSource = loadDataGridDetails();
            dgvLoanBalance.ItemsSource = loadDataGridLoanBalance();
        }

        private List<LoanModel> loadDataGridDetails()
        {
            List<LoanModel> lstLoanModel = new List<LoanModel>();
            LoanModel loanModel = new LoanModel();

            queryString = "SELECT dbspa.tblloans.ID, therapistID, dbspa.tbltherapist.description, loanamount, loandate FROM (dbspa.tblloans INNER JOIN " +
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

        private List<LoanModel> loadDataGridLoanBalance()
        {
            List<LoanModel> lstloanBalance = new List<LoanModel>();
            LoanModel loanBalance = new LoanModel();

            queryString = "SELECT dbspa.tblloans.ID, therapistID, dbspa.tbltherapist.description, Count(*) as cnt " +
                "FROM(dbspa.tblloans INNER JOIN dbspa.tbltherapist ON dbspa.tblloans.therapistID = dbspa.tbltherapist.ID) " +
                "WHERE dbspa.tblloans.isDeleted = 0 AND dbspa.tblloans.ifPaid = 0 GROUP BY therapistID";

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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            LoansDetailsWindow ldw = new LoansDetailsWindow(this);
            ldw.ShowDialog();
        }

        private void btnViewConsumableDetails_Click(object sender, RoutedEventArgs e)
        {
            LoanModel loanMod = dgvLoanBalance.SelectedItem as LoanModel;
            LoanBalanceWindow lbw = new LoanBalanceWindow(this, loanMod);
            lbw.ShowDialog();
        }

        private void btnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            LoanModel loan = dgvLoans.SelectedItem as LoanModel;
            if(loan != null)
            {
                LoanHistoryWindow lhw = new LoanHistoryWindow(this, loan);
                lhw.ShowDialog();
            }else
            {
                MessageBox.Show("Please select record!");
            }
            
        }
    }
}

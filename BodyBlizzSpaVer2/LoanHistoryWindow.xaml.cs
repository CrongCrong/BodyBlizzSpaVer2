using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for LoanHistoryWindow.xaml
    /// </summary>
    public partial class LoanHistoryWindow : MetroWindow
    {
        public LoanHistoryWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        LoansWindow loansWindow;
        LoanModel loanModel;
        string queryString = "";
        List<string> parameters;

        public LoanHistoryWindow(LoansWindow lw, LoanModel lm)
        {
            loansWindow = lw;
            loanModel = lm;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgvLoanHistory.ItemsSource = loadDataGridDetails(loanModel.ID);
        }

        private List<LoanModel> loadDataGridDetails(string strLoanID)
        {
            List<LoanModel> lstLoanHistory = new List<LoanModel>();
            LoanModel loanM;
            queryString = "SELECT dbspa.tblloanbalance.therapistID, dbspa.tbltherapist.description, amount, datepaid FROM ((dbspa.tblloanbalance " +
                "INNER JOIN dbspa.tbltherapist ON dbspa.tblloanbalance.therapistID = dbspa.tbltherapist.ID INNER JOIN dbspa.tblloans ON " +
                "dbspa.tblloanbalance.loanID = dbspa.tblloans.ID)) WHERE dbspa.tblloanbalance.isDeleted = 0 AND dbspa.tblloanbalance.loanID = ?";

            parameters = new List<string>();
            parameters.Add(strLoanID);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                loanM = new LoanModel();
                loanM.TherapistID = reader["therapistID"].ToString();
                loanM.Therapist = reader["description"].ToString();
                loanM.LoanAmount = reader["amount"].ToString();
                DateTime dte = DateTime.Parse(reader["datepaid"].ToString());
                loanM.DatePaid = dte.ToShortDateString();
                lstLoanHistory.Add(loanM);
            }

            conDB.closeConnection();

            return lstLoanHistory;
        }
    }
}

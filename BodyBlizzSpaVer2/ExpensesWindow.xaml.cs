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
    /// Interaction logic for ExpensesWindow.xaml
    /// </summary>
    public partial class ExpensesWindow : MetroWindow
    {
        public ExpensesWindow()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadDataGridDetails();
        }

        private void loadDataGridDetails()
        {
            List<ExpensesModel> lstExpensesModel = new List<ExpensesModel>();
            ExpensesModel expensesModel = new ExpensesModel();

            string queryString = "SELECT ID, date, description, cashout FROM dbspa.tblexpenses WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while(reader.Read())
            {
                expensesModel.ID = reader["ID"].ToString();
                DateTime dte = DateTime.Parse(reader["date"].ToString());
                expensesModel.Date = dte.ToShortDateString();
                expensesModel.Description = reader["description"].ToString();
                expensesModel.CashOut = reader["cashout"].ToString();
                lstExpensesModel.Add(expensesModel);
                expensesModel = new ExpensesModel();
            }
            conDB.closeConnection();
            dgvExpenses.ItemsSource = lstExpensesModel;

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ExpensesDetails expensesDetails = new ExpensesDetails(this);
            expensesDetails.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ExpensesModel em = dgvExpenses.SelectedItem as ExpensesModel;

            ExpensesDetails expenseDetails = new ExpensesDetails(this, em);
            expenseDetails.ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

 
    }
}

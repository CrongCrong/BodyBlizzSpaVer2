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
    /// Interaction logic for ExpensesDetails.xaml
    /// </summary>
    public partial class ExpensesDetails : MetroWindow
    {
        public ExpensesDetails()
        {
            InitializeComponent();
        }

        ConnectionDB conDB = new ConnectionDB();
        ExpensesWindow expenseWindow;
        ExpensesModel expenseModel;

        public ExpensesDetails(ExpensesWindow ew)
        {
            expenseWindow = ew;
            InitializeComponent();
        }

        public ExpensesDetails(ExpensesWindow ew, ExpensesModel em)
        {
            expenseModel = em;
            expenseWindow = ew;
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            dateExpenses.Text = DateTime.Now.ToShortDateString();
            if(expenseModel != null)
            {
                dateExpenses.Text = expenseModel.Date;
                txtDescription.Text = expenseModel.Description;
                txtCashout.Text = expenseModel.CashOut;
                btnUpdate.Visibility = Visibility.Visible;
            }

        }

        private bool checkFields()
        {
            bool ifAllCorrect = false;

            if (string.IsNullOrEmpty(dateExpenses.Text))
            {
                MessageBox.Show("Please select Date");

            }else if(string.IsNullOrEmpty(txtDescription.Text))
            {
                MessageBox.Show("Please provide Notes!");
            }else if(string.IsNullOrEmpty(txtCashout.Text))
            {
                MessageBox.Show("Please input Cash Out");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void loadDataGridDetails()
        {
            List<ExpensesModel> lstExpensesModel = new List<ExpensesModel>();
            ExpensesModel expensesModel = new ExpensesModel();

            string queryString = "SELECT ID, date, description, cashout FROM dbspa.tblexpenses WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
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
            expenseWindow.dgvExpenses.ItemsSource = lstExpensesModel;

        }

        private void insertExpenseRecord()
        {
            string queryString = "INSERT INTO dbspa.tblexpenses(date, description, cashout, isDeleted) VALUES (?,?,?,?)";
            List<string> parameters = new List<string>();

            DateTime date = DateTime.Parse(dateExpenses.Text);
            parameters.Add(date.Year + "/" + date.Month + "/" + date.Day);

            parameters.Add(txtDescription.Text);
            parameters.Add(txtCashout.Text);
            parameters.Add("0");

            conDB.getSelectConnection(queryString, parameters);
            conDB.closeConnection();           
        }

        private void updateRecord()
        {
            string queryString = "UPDATE dbspa.tblexpenses SET date = ?, description = ?, cashout = ? WHERE ID = ?";
            List<string> parameters = new List<string>();

            parameters.Add(dateExpenses.Text);
            parameters.Add(txtDescription.Text);
            parameters.Add(txtCashout.Text);
            parameters.Add(expenseModel.ID);

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(checkFields())
            {
                insertExpenseRecord();
                loadDataGridDetails();
                MessageBox.Show("RECORD SAVED SUCCESSFULLY!");
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(checkFields())
            {
                updateRecord();
                loadDataGridDetails();
                MessageBox.Show("RECORD UPDATED SUCCESSFULLY!");
                this.Close();
            }
        }

        private void txtCashout_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

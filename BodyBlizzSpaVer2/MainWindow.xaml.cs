using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        ConnectionDB conDB = new ConnectionDB();
        string queryString = "";
        List<string> parameters;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void LoginUser()
        {
            User user = new User();
            try
            {
                user = getLoginDetails(txtUsername.Text, txtPassword.Password);

                if (user.Username.Equals(txtUsername.Text) && user.Password.Equals(txtPassword.Password))
                {
                    conDB.writeLogFile("LOG-IN SUCCESSFULL! USERNAME: " + user.Username);
                    MainMenu menu = new MainMenu(user, this);
                    this.Hide();
                    menu.Show();
                    txtUsername.Text = "";
                    txtPassword.Password = "";

                }
                else
                {
                    conDB.writeLogFile("LOG-IN FAILED! USERNAME: " + txtUsername.Text);
                    MessageBox.Show("LOG IN FAILED!!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LOG IN FAILED! - Incorret Username/Password");
            }

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginUser();
        }

        private User getLoginDetails(string username, string password)
        {
            User user = new User();

            queryString = "SELECT ID, firstName, LastName, userType, username, password FROM dbspa.tblUser WHERE (username = ?) AND (password = ?)";
            parameters = new List<string>();

            parameters.Add(username);
            parameters.Add(password);


            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                user.Id = Convert.ToInt32(reader["ID"].ToString());
                user.FirstName = reader["firstName"].ToString();
                user.LastName = reader["LastName"].ToString();
                user.Type = Convert.ToInt32(reader["userType"].ToString());
                user.Username = reader["username"].ToString();
                user.Password = reader["password"].ToString();
            }

            conDB.closeConnection();


            return user;
        }

        private void LogIn_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            txtUsername.Text = "";
            txtPassword.Password = "";
        }

        private void LogIn_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && e.Key == Key.Enter)
            {
                LoginUser();
            }
        }
    }
}

using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : MetroWindow
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        User user;
        MainWindow logInForm;

        public MainMenu(User usr, MainWindow logIn)
        {
            user = usr;
            logInForm = logIn;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(user.Type > 1)
            {
                btnTherapist.Visibility = Visibility.Hidden;
                btnAttendance.Visibility = Visibility.Hidden;
                btnSettings.Margin = new Thickness(226, 28, 0, 0);
                btnLogout.Margin = new  Thickness(406, 28, 0, 0);
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logInForm.Show();
        }

        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            ClientForm clientForm = new ClientForm(user);
            clientForm.ShowDialog();
        }

        private void btnAttendance_Click(object sender, RoutedEventArgs e)
        {
            AttendanceForm af = new AttendanceForm();
            af.ShowDialog();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(user);
            settingsWindow.ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            logInForm.Show();
            this.Close();
        }

        private void btnTherapist_Click(object sender, RoutedEventArgs e)
        {
            Therapist t = new Therapist();
            t.ShowDialog();
        }
    }
}

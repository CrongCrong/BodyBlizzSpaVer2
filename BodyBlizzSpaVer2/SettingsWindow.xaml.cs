using BodyBlizzSpaVer2.Classes;
using MahApps.Metro.Controls;
using System.Windows;

namespace BodyBlizzSpaVer2
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        User user;

        public SettingsWindow(User usr)
        {
            user = usr;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(user.Type > 1)
            {
                btnServiceMode.Visibility = Visibility.Hidden;
                label.Visibility = Visibility.Hidden;
                btnServiceType.Visibility = Visibility.Hidden;
                label1.Visibility = Visibility.Hidden;
                btnCommission.Visibility = Visibility.Hidden;
                label2.Visibility = Visibility.Hidden;
                btnDiscount.Visibility = Visibility.Hidden;
                label3.Visibility = Visibility.Hidden;
                btnProducts.Visibility = Visibility.Hidden;
                label4.Visibility = Visibility.Hidden;
                btnDiscount.Visibility = Visibility.Hidden;
                label5.Visibility = Visibility.Hidden;
                btnExpenses.Visibility = Visibility.Hidden;
                
                btnLoyaltyCard.Margin = new Thickness(43, 43, 0, 0);
                label6.Margin = new Thickness(26, 119, 0, 0);

                btnPromo.Visibility = Visibility.Hidden;
                label7.Visibility = Visibility.Hidden;
                btnReports.Visibility = Visibility.Hidden;
                label8.Visibility = Visibility.Hidden;

                btnConsumables.Margin = new Thickness(252, 37, 0, 0);
                label9.Margin = new Thickness(238, 119, 0, 0);

            
            }
        }

        private void btnServiceMode_Click(object sender, RoutedEventArgs e)
        {
            ServiceModeWindow serviceModeWin = new ServiceModeWindow();
            serviceModeWin.ShowDialog();
        }

        private void btnServiceType_Click(object sender, RoutedEventArgs e)
        {
            ServiceTypeWindow serviceTypeWin = new ServiceTypeWindow();
            serviceTypeWin.ShowDialog();
        }

        private void btnCommission_Click(object sender, RoutedEventArgs e)
        {
            CommissionWindow commissionWin = new CommissionWindow();
            commissionWin.ShowDialog();
        }

        private void btnDiscount_Click(object sender, RoutedEventArgs e)
        {
            DiscountWindow discountWin = new DiscountWindow();
            discountWin.ShowDialog();
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productsWin = new ProductsWindow();
            productsWin.ShowDialog();
        }

        private void btnExpenses_Click(object sender, RoutedEventArgs e)
        {
            ExpensesWindow expensesWin = new ExpensesWindow();
            expensesWin.ShowDialog();
        }

        private void btnLoyaltyCard_Click(object sender, RoutedEventArgs e)
        {
            LoyaltyCardWindow loyalC = new LoyaltyCardWindow();
            loyalC.ShowDialog();
        }

        private void btnPromo_Click(object sender, RoutedEventArgs e)
        {
            PromoWindow promoWin = new PromoWindow();
            promoWin.ShowDialog();
        }

        private void btnPromoService_Click(object sender, RoutedEventArgs e)
        {
            PromoServiceWindow promoService = new PromoServiceWindow();
            promoService.ShowDialog();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            PrintExpensesWindow pew = new PrintExpensesWindow();
            pew.ShowDialog();
        }

        private void btnConsumables_Click(object sender, RoutedEventArgs e)
        {
            ConsumableWindow consumableWindow = new ConsumableWindow();
            consumableWindow.ShowDialog();
        }
    }
}

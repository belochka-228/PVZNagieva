using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_Nagieva.DTO;
using WPF_Nagieva.Pages;
using WPF_Nagieva.Services;

namespace WPF_Nagieva
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new CatalogPage());
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CatalogPage());
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new CartWindow();
            cartWindow.ShowDialog();
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new AdminWindow();
            adminWindow.ShowDialog();
        }

        private void BtnFavorites_Click(object sender, RoutedEventArgs e)
        {
            var favWindow = new FavoritesWindow();
            favWindow.ShowDialog();
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryWindow();
            historyWindow.ShowDialog();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (BtnLogin.Content.ToString() == "Выйти")
            {
                ApiClient.Logout();

                BtnAdmin.Visibility = Visibility.Collapsed;
                BtnLogin.Content = "Войти";

                MessageBox.Show("Вы вышли из аккаунта.");
                return;
            }

            LoginWindow loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                BtnLogin.Content = "Выйти";
                if (ApiClient.CurrentUserRole == "Admin")
                {
                    BtnAdmin.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
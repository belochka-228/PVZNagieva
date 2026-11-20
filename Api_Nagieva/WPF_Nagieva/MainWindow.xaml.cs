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
        private bool IsUserLoggedIn()
        {
            if (ApiClient.Client.DefaultRequestHeaders.Authorization == null)
            {
                MessageBox.Show("Для этого действия необходимо войти в аккаунт!", "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserLoggedIn()) return;

            CartWindow cartWindow = new CartWindow();
            cartWindow.ShowDialog();

            MainFrame.Navigate(new CatalogPage());
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserLoggedIn()) return;

            var adminWindow = new AdminWindow();
            adminWindow.ShowDialog();
        }

        private void BtnFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserLoggedIn()) return;

            var favWindow = new FavoritesWindow();
            favWindow.ShowDialog();

            MainFrame.Navigate(new CatalogPage());
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserLoggedIn()) return;

            var historyWindow = new HistoryWindow();
            historyWindow.ShowDialog();

            MainFrame.Navigate(new CatalogPage());
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
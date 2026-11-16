using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using WPF_Nagieva.DTO;
using WPF_Nagieva.Services;

namespace WPF_Nagieva.Pages
{
    public partial class FavoritesWindow : Window
    {
        public FavoritesWindow()
        {
            InitializeComponent();
            Loaded += FavoritesWindow_Loaded;
        }

        private async void FavoritesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFavoritesAsync();
        }

        private async System.Threading.Tasks.Task LoadFavoritesAsync()
        {
            try
            {
                LvFavorites.ItemsSource = null;
                var favorites = await ApiClient.Client.GetFromJsonAsync<List<FavoriteResponse>>("api/Favorites");
                if (favorites != null)
                {
                    LvFavorites.ItemsSource = favorites;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки избранного: " + ex.Message);
            }
        }

        private async void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as FavoriteResponse;
            if (item == null) return;

            var response = await ApiClient.Client.DeleteAsync($"api/Favorites/{item.ProductId}");
            if (response.IsSuccessStatusCode)
            {
                await LoadFavoritesAsync();
            }
        }

        private async void BtnMoveToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as FavoriteResponse;
            if (item == null) return;

            try
            {
                var response = await ApiClient.Client.PostAsync($"api/Favorites/{item.ProductId}/move-to-cart", null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Товар '{item.ProductName}' перенесен в корзину!");
                    await LoadFavoritesAsync();
                }
                else
                {
                    MessageBox.Show("Не удалось перенести (возможно, нет на складе).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_Nagieva.DTO;
using WPF_Nagieva.Services;

namespace WPF_Nagieva
{
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
            Loaded += CatalogPage_Loaded;
        }

        private async void CatalogPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync(string search = "")
        {
            try
            {
                LvProducts.ItemsSource = null;
                string url = string.IsNullOrWhiteSpace(search) ? "api/Products" : $"api/Products?search={search}";

                var response = await ApiClient.Client.GetFromJsonAsync<List<ProductResponse>>(url);

                if (response != null)
                {
                    LvProducts.ItemsSource = response;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки товаров: \n" + ex.Message);
            }
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadProductsAsync(TxtSearch.Text);
        }

        private async void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.DataContext as ProductResponse;

            if (product == null) return;

            try
            {
                var request = new { ProductId = product.Id, Quantity = 1 };
                var response = await ApiClient.Client.PostAsJsonAsync("api/Cart/add", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!");
                }
                else
                {
                    MessageBox.Show("Ошибка добавления! Проверьте, авторизованы ли вы.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
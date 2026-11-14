using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using WPF_Nagieva.DTO;
using WPF_Nagieva.Services;

namespace WPF_Nagieva
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
            Loaded += CartWindow_Loaded;
        }

        private async void CartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCartAsync();
        }

        private async System.Threading.Tasks.Task LoadCartAsync()
        {
            try
            {
                var cart = await ApiClient.Client.GetFromJsonAsync<List<CartItemResponse>>("api/Cart");

                LvCart.ItemsSource = cart;

                if (cart != null)
                {
                    decimal total = cart.Sum(item => item.TotalPrice);
                    TxtTotal.Text = $"Итого: {total:C}";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки корзины: " + ex.Message);
            }
        }

        private async void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as CartItemResponse;

            if (item == null) return;

            var response = await ApiClient.Client.DeleteAsync($"api/Cart/{item.ProductId}");

            if (response.IsSuccessStatusCode)
            {
                await LoadCartAsync();
            }
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Оформление заказа!");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_Nagieva.DTO;
using WPF_Nagieva.Pages;
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

        private async Task LoadCartAsync()
        {
            try
            {
                LvCart.ItemsSource = null;
                var cart = await ApiClient.Client.GetFromJsonAsync<List<CartItemResponse>>("api/Cart");

                if (cart != null)
                {
                    LvCart.ItemsSource = cart;

                    decimal total = cart.Sum(item => item.TotalPrice);
                    TxtTotal.Text = $"Итого: {total:C}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки корзины: " + ex.Message);
            }
        }

        private async void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as CartItemResponse;

            if (item == null) return;

            try
            {
                var request = new { ProductId = item.ProductId, Quantity = item.Quantity - 1 };
                var response = await ApiClient.Client.PutAsJsonAsync("api/Cart/update", request);

                if (response.IsSuccessStatusCode)
                {
                    await LoadCartAsync();
                }
                else
                {
                    MessageBox.Show("Не удалось изменить количество (возможно, достигнут лимит).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as CartItemResponse;

            if (item == null) return;

            try
            {
                var request = new { ProductId = item.ProductId, Quantity = item.Quantity + 1 };
                var response = await ApiClient.Client.PutAsJsonAsync("api/Cart/update", request);

                if (response.IsSuccessStatusCode)
                {
                    await LoadCartAsync();
                }
                else
                {
                    MessageBox.Show("На складе не хватает товара!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private async void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (LvCart.Items.Count == 0)
            {
                MessageBox.Show("Ваша корзина пуста! Добавьте товары перед оформлением.", "Внимание");
                return;
            }

            decimal total = 0;
            foreach (var item in LvCart.Items)
            {
                if (item is CartItemResponse cartItem)
                {
                    total += cartItem.TotalPrice;
                }
            }

            CheckoutWindow checkout = new CheckoutWindow(total);

            if (checkout.ShowDialog() == true)
            {
                MessageBox.Show("Спасибо за покупку! Ваш заказ оформлен.", "Успех");
                await LoadCartAsync();
            }
        }
    }
}
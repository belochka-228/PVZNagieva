using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using WPF_Nagieva.DTO;   
using WPF_Nagieva.Services;

namespace WPF_Nagieva.Pages
{
    public partial class CheckoutWindow : Window
    {
        private decimal _totalAmount;
        private List<OrderItemRequest> _specificItems; 

        public CheckoutWindow(decimal totalAmount) : this(totalAmount, null) { }

        public CheckoutWindow(decimal totalAmount, List<OrderItemRequest> specificItems)
        {
            InitializeComponent();
            _totalAmount = totalAmount;
            _specificItems = specificItems;

            TxtTotalInfo.Text = $"Сумма заказа: {_totalAmount:C}";

            if (_totalAmount < 600)
            {
                RbCourier.IsEnabled = false;
                RbPickup.IsChecked = true;
                MessageBox.Show("Сумма заказа меньше 600 рублей. Доступен только самовывоз.");
            }
            else
            {
                RbCourier.IsChecked = true;
            }

            Loaded += CheckoutWindow_Loaded;
        }

        private async void CheckoutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var points = await ApiClient.Client.GetFromJsonAsync<List<PickupPointResponse>>("api/PickupPoints");
                if (points != null)
                {
                    CbPickupPoints.ItemsSource = points;
                    if (points.Count > 0) CbPickupPoints.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки ПВЗ: " + ex.Message);
            }
        }

        private void DeliveryType_Changed(object sender, RoutedEventArgs e)
        {
            if (PanelPickup == null || PanelCourier == null) return;

            if (RbPickup.IsChecked == true)
            {
                PanelPickup.Visibility = Visibility.Visible;
                PanelCourier.Visibility = Visibility.Collapsed;
            }
            else
            {
                PanelPickup.Visibility = Visibility.Collapsed;
                PanelCourier.Visibility = Visibility.Visible;
            }
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new CreateOrderRequest
                {
                    DeliveryType = RbPickup.IsChecked == true ? "Pickup" : "Courier",
                    PickupPointId = RbPickup.IsChecked == true ? (int?)CbPickupPoints.SelectedValue : null,
                    DeliveryAddress = RbCourier.IsChecked == true ? TxtAddress.Text : "",
                    Items = _specificItems
                };

                var response = await ApiClient.Client.PostAsJsonAsync("api/Orders", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Заказ успешно оформлен!", "Успех");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Ошибка оформления: " + error, "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}

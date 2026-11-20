using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Nagieva.DTO;
using WPF_Nagieva.Services;
using System.Net.Http.Json;


namespace WPF_Nagieva.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            Loaded += AdminWindow_Loaded;
        }

        private async void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrdersAsync();
        }

        private async void BtnCreateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(TxtPrice.Text, out decimal price) ||
                    !int.TryParse(TxtCategoryId.Text, out int catId) ||
                    !int.TryParse(TxtStock.Text, out int stock))
                {
                    MessageBox.Show("Проверьте ввод цены, категории и остатков (должны быть числа).");
                    return;
                }

                var request = new
                {
                    Name = TxtName.Text,
                    Description = TxtDesc.Text,
                    Price = price,
                    ImagePath = TxtImage.Text,
                    CategoryId = catId,
                    StockQuantity = stock,
                    PackQuantity = (int?)null,
                    IsAvailable = ChkIsAvailable.IsChecked ?? false
                };

                var response = await ApiClient.Client.PostAsJsonAsync("api/Products", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Товар успешно добавлен!", "Успех");
                    TxtName.Text = "";
                    TxtPrice.Text = "";
                    TxtDesc.Text = "";
                }
                else
                {
                    MessageBox.Show("Ошибка добавления. Возможно, неверный ID категории.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                LvAllOrders.ItemsSource = null;
                var orders = await ApiClient.Client.GetFromJsonAsync<List<OrderResponse>>("api/Orders");
                if (orders != null)
                {
                    LvAllOrders.ItemsSource = orders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заказов: " + ex.Message);
            }
        }

        private async void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = LvAllOrders.SelectedItem as OrderResponse;
            if (selectedOrder == null)
            {
                MessageBox.Show("Сначала выберите заказ в списке!");
                return;
            }

            var selectedStatus = CbNewStatus.SelectedItem as ComboBoxItem;
            if (selectedStatus == null)
            {
                MessageBox.Show("Выберите новый статус из выпадающего списка!");
                return;
            }

            string newStatus = selectedStatus.Content.ToString();

            try
            {
                var response = await ApiClient.Client.PatchAsJsonAsync($"api/Orders/{selectedOrder.Id}/status", newStatus);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Статус заказа #{selectedOrder.Id} успешно изменен на '{newStatus}'!");
                    await LoadOrdersAsync();
                }
                else
                {
                    MessageBox.Show("Не удалось изменить статус.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private async void BtnCreatePickupPoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new
                {
                    Name = TxtPpName.Text,
                    Address = TxtPpAddress.Text
                };

                var response = await ApiClient.Client.PostAsJsonAsync("api/PickupPoints", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("ПВЗ успешно создан!", "Успех");
                    TxtPpName.Text = "";
                    TxtPpAddress.Text = "";
                }
                else
                {
                    MessageBox.Show("Ошибка создания ПВЗ.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private async void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCategoryName.Text))
            {
                MessageBox.Show("Введите название категории!");
                return;
            }

            try
            {
                var request = new { Name = TxtCategoryName.Text };
                var response = await ApiClient.Client.PostAsJsonAsync("api/Categories", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Категория успешно создана!", "Успех");
                    TxtCategoryName.Text = "";
                }
                else
                {
                    MessageBox.Show("Ошибка создания категории (возможно, такое имя уже есть).", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}

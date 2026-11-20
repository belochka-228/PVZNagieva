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
            await LoadAllDataAsync();
        }
        private async Task LoadAllDataAsync()
        {
            try
            {
                var products = await ApiClient.Client.GetFromJsonAsync<List<ProductResponse>>("api/Products");
                if (products != null) DgProducts.ItemsSource = products;

                var orders = await ApiClient.Client.GetFromJsonAsync<List<OrderResponse>>("api/Orders/all");
                if (orders != null) LbAdminOrders.ItemsSource = orders;

                var statuses = await ApiClient.Client.GetFromJsonAsync<List<OrderStatusResponse>>("api/Orders/statuses");
                if (statuses != null) CbNewStatus.ItemsSource = statuses;

                var categories = await ApiClient.Client.GetFromJsonAsync<List<CategoryResponse>>("api/Categories");
                if (categories != null)
                {
                    DgCategories.ItemsSource = categories;

                    var categoryColumn = DgProducts.Columns[5] as DataGridComboBoxColumn;
                    if (categoryColumn != null)
                    {
                        categoryColumn.ItemsSource = categories;
                    }
                }

                var pickupPoints = await ApiClient.Client.GetFromJsonAsync<List<PickupPointResponse>>("api/PickupPoints");
                if (pickupPoints != null) DgPickupPoints.ItemsSource = pickupPoints;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private async void BtnSaveGridChanges_Click(object sender, RoutedEventArgs e)
        {
            var products = DgProducts.ItemsSource as List<ProductResponse>;
            if (products == null) return;

            int success = 0, errors = 0;
            foreach (var p in products)
            {
                var request = new
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImagePath = p.ImagePath,
                    CategoryId = p.CategoryId,
                    StockQuantity = p.StockQuantity,
                    PackQuantity = p.PackQuantity,
                    IsAvailable = p.IsAvailable
                };
                var response = await ApiClient.Client.PutAsJsonAsync($"api/Products/{p.Id}", request);
                if (response.IsSuccessStatusCode) success++; else errors++;
            }
            MessageBox.Show($"Сохранено: {success}. Ошибок: {errors}.");
            await LoadAllDataAsync();
        }

        private async void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new
                {
                    Name = "Новый товар",
                    Description = "",
                    Price = 0,
                    ImagePath = "default.jpg",
                    CategoryId = 1,
                    StockQuantity = 0,
                    PackQuantity = (int?)null,
                    IsAvailable = true
                };

                var response = await ApiClient.Client.PostAsJsonAsync("api/Products", request);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAllDataAsync();
                }
                else
                {
                    MessageBox.Show("Ошибка создания товара.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private async void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var order = LbAdminOrders.SelectedItem as OrderResponse;
            var status = CbNewStatus.SelectedItem as OrderStatusResponse;

            if (order == null || status == null)
            {
                MessageBox.Show("Выберите заказ в списке и новый статус!");
                return;
            }

            var response = await ApiClient.Client.PatchAsJsonAsync($"api/Orders/{order.Id}/status", status.Name);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Статус изменен!");
                await LoadAllDataAsync();
            }
            else MessageBox.Show("Ошибка изменения статуса.");
        }

        private async void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCategoryName.Text)) return;

            var request = new { Name = TxtCategoryName.Text };
            var response = await ApiClient.Client.PostAsJsonAsync("api/Categories", request);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Категория создана!");
                TxtCategoryName.Text = "";
                await LoadAllDataAsync();
            }
            else MessageBox.Show("Ошибка (возможно, категория уже есть).");
        }
        private async void BtnSaveCategories_Click(object sender, RoutedEventArgs e)
        {
            var categories = DgCategories.ItemsSource as List<CategoryResponse>;
            if (categories == null) return;

            int success = 0;
            foreach (var cat in categories)
            {
                var response = await ApiClient.Client.PutAsJsonAsync($"api/Categories/{cat.Id}", cat.Name);
                if (response.IsSuccessStatusCode) success++;
            }
            MessageBox.Show($"Сохранено категорий: {success}");
            await LoadAllDataAsync();
        }
        private async void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var cat = DgCategories.SelectedItem as CategoryResponse;
            if (cat == null) { MessageBox.Show("Выберите категорию!"); return; }

            var result = MessageBox.Show($"Удалить категорию '{cat.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var response = await ApiClient.Client.DeleteAsync($"api/Categories/{cat.Id}");
                if (response.IsSuccessStatusCode) await LoadAllDataAsync();
                else MessageBox.Show("Нельзя удалить (возможно, к ней привязаны товары).");
            }
        }

        private async void BtnCreatePickupPoint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPpName.Text) || string.IsNullOrWhiteSpace(TxtPpAddress.Text)) return;

            var request = new { Name = TxtPpName.Text, Address = TxtPpAddress.Text };
            var response = await ApiClient.Client.PostAsJsonAsync("api/PickupPoints", request);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("ПВЗ создан!");
                TxtPpName.Text = "";
                TxtPpAddress.Text = "";
                await LoadAllDataAsync();
            }
            else MessageBox.Show("Ошибка создания ПВЗ.");
        }

        private async void BtnSavePickupPoints_Click(object sender, RoutedEventArgs e)
        {
            var points = DgPickupPoints.ItemsSource as List<PickupPointResponse>;
            if (points == null) return;

            int success = 0;
            foreach (var pp in points)
            {
                var request = new { Name = pp.Name, Address = pp.Address };
                var response = await ApiClient.Client.PutAsJsonAsync($"api/PickupPoints/{pp.Id}", request);
                if (response.IsSuccessStatusCode) success++;
            }
            MessageBox.Show($"Сохранено ПВЗ: {success}");
            await LoadAllDataAsync();
        }
        private async void BtnDeletePickupPoint_Click(object sender, RoutedEventArgs e)
        {
            var pp = DgPickupPoints.SelectedItem as PickupPointResponse;
            if (pp == null) { MessageBox.Show("Выберите ПВЗ!"); return; }

            var result = MessageBox.Show($"Удалить ПВЗ '{pp.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var response = await ApiClient.Client.DeleteAsync($"api/PickupPoints/{pp.Id}");
                if (response.IsSuccessStatusCode) await LoadAllDataAsync();
                else MessageBox.Show("Нельзя удалить (возможно, к нему привязаны заказы).");
            }
        }
    }
}
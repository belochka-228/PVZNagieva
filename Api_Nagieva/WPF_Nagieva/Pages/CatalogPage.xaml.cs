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
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
            Loaded += CatalogPage_Loaded;
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
        private async void CatalogPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                LvProducts.ItemsSource = null;
                string search = TxtSearch.Text;
                int? categoryId = null;

                if (CmbCategory.SelectedItem is CategoryResponse selectedCat && selectedCat.Id > 0)
                {
                    categoryId = selectedCat.Id;

                }
                decimal? minPrice = null;
                if (decimal.TryParse(TxtMinPrice.Text, out decimal min)) minPrice = min;

                decimal? maxPrice = null;
                if (decimal.TryParse(TxtMaxPrice.Text, out decimal max)) maxPrice = max;


                string url = "api/Products?";
                if (!string.IsNullOrWhiteSpace(search)) 
                    url += $"search={search}&";
                if (categoryId.HasValue) 
                    url += $"categoryId={categoryId.Value}&";
                if (minPrice.HasValue) 
                    url += $"minPrice={minPrice.Value}&";
                if (maxPrice.HasValue) 
                    url += $"maxPrice={maxPrice.Value}&";


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
        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await ApiClient.Client.GetFromJsonAsync<List<CategoryResponse>>("api/Categories");
                if (categories != null)
                {
                    categories.Insert(0, new CategoryResponse { Id = 0, Name = "Все категории" });

                    CmbCategory.ItemsSource = categories;
                    CmbCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message);
            }
        }

        private async void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserLoggedIn()) return;

            var button = sender as Button;
            var product = button?.DataContext as ProductResponse;

            if (product == null) return;

            if (!product.IsAvailable || product.StockQuantity < 1)
            {
                MessageBox.Show("Этот товар недоступен для покупки (нет в наличии)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var request = new { ProductId = product.Id, Quantity = 1 };
                var response = await ApiClient.Client.PostAsJsonAsync("api/Cart/add", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void BtnBuyNow_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserLoggedIn()) return;

            var button = sender as Button;
            var product = button?.DataContext as ProductResponse;
            if (product == null) return;

            if (!product.IsAvailable || product.StockQuantity < 1)
            {
                MessageBox.Show("Товара нет в наличии!");
                return;
            }

            var items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = product.Id, Quantity = 1 }
            };

            var checkout = new CheckoutWindow(product.Price, items);
            if (checkout.ShowDialog() == true)
            {
                MessageBox.Show("Покупка совершена!");
            }
        }
        private async void BtnReviews_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.DataContext as ProductResponse;
            if (product == null) return;

            var reviewsWindow = new ReviewsWindow(product.Id, product.Name);
            reviewsWindow.ShowDialog();
            await LoadProductsAsync();
        }

        private async void BtnAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserLoggedIn()) return;
            var button = sender as Button;
            var product = button?.DataContext as ProductResponse;

            if (product == null) return;

            try
            {
                var response = await ApiClient.Client.PostAsync($"api/Favorites/{product.Id}", null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Товар '{product.Name}' добавлен в избранное!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private async void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadProductsAsync();

        }
        private async void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            await LoadProductsAsync();
        }

        private async void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            await LoadProductsAsync();
        }
    }
}
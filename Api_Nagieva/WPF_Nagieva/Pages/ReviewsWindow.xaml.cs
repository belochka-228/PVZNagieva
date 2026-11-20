using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
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

namespace WPF_Nagieva.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        private int _productId;
        public ReviewsWindow(int productId, string productName)
        {
            InitializeComponent();
            _productId = productId;
            TxtTitle.Text = $"Отзывы: {productName}";
            Loaded += ReviewsWindow_Loaded;
            
        }

        private async void ReviewsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApiClient.CurrentUserRole == "Admin" || ApiClient.Client.DefaultRequestHeaders.Authorization == null)
            {
                PanelAddReview.Visibility = Visibility.Collapsed;
            }
            await LoadReviewsAsync();
        }

        private async Task LoadReviewsAsync()
        {
            try
            {
                LvReviews.ItemsSource = null;
                var reviews = await ApiClient.Client.GetFromJsonAsync<List<ReviewResponse>>($"api/Reviews/product/{_productId}");

                if (reviews != null)
                {
                    LvReviews.ItemsSource = reviews;

                    if (ApiClient.CurrentUserRole != "Admin")
                    {
                        LvReviews.UpdateLayout();

                        foreach (var item in reviews)
                        {
                            var container = LvReviews.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                            if (container != null)
                            {
                                var btn = FindVisualChild<Button>(container);
                                if (btn != null) btn.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки отзывов: " + ex.Message);
            }
        }

        private async void BtnAddReview_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtComment.Text))
            {
                MessageBox.Show("Напишите комментарий!");
                return;
            }

            try
            {
                int rating = int.Parse(((ComboBoxItem)CbRating.SelectedItem).Content.ToString());

                var request = new CreateReviewRequest
                {
                    ProductId = _productId,
                    Rating = rating,
                    Comment = TxtComment.Text
                };

                var response = await ApiClient.Client.PostAsJsonAsync("api/Reviews", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Спасибо за отзыв!");
                    TxtComment.Text = ""; 
                    await LoadReviewsAsync(); 
                }
                else
                {
                    MessageBox.Show("Ошибка при отправке отзыва.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private async void BtnDeleteReview_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var review = button?.DataContext as ReviewResponse;
            if (review == null) return;
            
            var result = MessageBox.Show("Вы уверены, что хотите удалить этот отзыв?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var response = await ApiClient.Client.DeleteAsync($"api/Reviews/{review.Id}");
                if (response.IsSuccessStatusCode)
                {
                    await LoadReviewsAsync();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить отзыв (возможно, у вас нет прав).");
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild) return typedChild;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}

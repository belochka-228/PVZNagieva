using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using WPF_Nagieva.DTO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Nagieva.Services;

namespace WPF_Nagieva.Pages
{
    /// <summary>
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
            Loaded += HistoryWindow_Loaded;
        }

        private async void HistoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrdersAsync(true);
        }

        private async void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            bool newestFirst = CbSort.SelectedIndex == 0;
            await LoadOrdersAsync(newestFirst);
        }

        private async System.Threading.Tasks.Task LoadOrdersAsync(bool newestFirst)
        {
            try
            {
                LvOrders.ItemsSource = null;
                var orders = await ApiClient.Client.GetFromJsonAsync<List<OrderResponse>>($"api/Orders?newestFirst={newestFirst.ToString().ToLower()}");

                if (orders != null)
                {
                    LvOrders.ItemsSource = orders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки истории: " + ex.Message);
            }
        }
    }
}
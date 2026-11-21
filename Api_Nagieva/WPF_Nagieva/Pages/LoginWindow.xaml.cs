using System;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Navigation;
using WPF_Nagieva.DTO;
using WPF_Nagieva.Services;

namespace WPF_Nagieva
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new { Login = TxtLogin.Text, Password = TxtPassword.Password };
                var response = await ApiClient.Client.PostAsJsonAsync("api/Auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null)
                    {
                        ApiClient.SetToken(result.AccessToken);
                        ApiClient.CurrentUserRole = result.Role;
                        MessageBox.Show($"Добро пожаловать, {result.Login}!\nРоль: {result.Role}", "Успех");
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private async void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new { Login = TxtLogin.Text, Password = TxtPassword.Password };
                var response = await ApiClient.Client.PostAsJsonAsync("api/Auth/register", request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Регистрация успешна!", "Успех");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Код: {response.StatusCode}\nОтвет сервера: {errorContent}", "Ошибка");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
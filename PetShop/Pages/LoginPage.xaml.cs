using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PetShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private int failedAttempts = 0;
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(LoginTextBox.Text))
                {
                    errors.AppendLine("Заполните логин");
                }
                if (string.IsNullOrEmpty(PasswordBox.Password))
                {
                    errors.AppendLine("Заполните пароль");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (failedAttempts > 0) 
                {
                    if (string.IsNullOrEmpty(CaptchaWriteBox.Text))
                    {
                        MessageBox.Show("Введите капчу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoadCaptcha();
                        return;
                    }

                    if (!CaptchaWriteBox.Text.Equals(CaptchaBox.Text, StringComparison.Ordinal))
                    {
                        MessageBox.Show("Неверная капча!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        CaptchaWriteBox.Text = "";
                        LoadCaptcha();
                        return;
                    }
                }

                if (Data.Trade2Entities.GetContext().User
                    .Any(d => d.UserLogin == LoginTextBox.Text && 
                    d.UserPassword == PasswordBox.Password))
                {
                    var user = Data.Trade2Entities.GetContext().User.Where(d => d.UserLogin == 
                    LoginTextBox.Text && 
                    d.UserPassword == PasswordBox.Password).FirstOrDefault();
                    Classes.Manager.User = user;
                    switch(user.Role.RoleName)
                    {
                        case "Администратор":
                            Classes.Manager.MainFrame.Navigate(new Pages.AdminLkPage());
                            break;
                        case "Клиент":
                            Classes.Manager.MainFrame.Navigate(new Pages.ClientLkPage());
                            break;
                        case "Менеджер":
                            Classes.Manager.MainFrame.Navigate(new Pages.ManagerLkPage());
                            break;
                    }
                    MessageBox.Show("Успех", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LabelCaptcha.Visibility = Visibility.Collapsed;
                    CaptchaBox.Visibility = Visibility.Collapsed;
                    CaptchaWriteBox.Visibility = Visibility.Collapsed;
                    CaptchaWriteBox.Text = "";
                    failedAttempts = 0;
                }
                else
                {
                    MessageBox.Show("Учетная запись не найдена", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    failedAttempts++;
                    LoadCaptcha();
                    ShowCaptchaFields();
                    if(failedAttempts > 1)
                    {
                        LoginButton.IsEnabled = false;
                        await Task.Delay(10000);
                        LoginButton.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCaptcha()
        {
            string allowChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            string captcha = "";
            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                captcha += allowChars[random.Next(allowChars.Length)];
            }

            CaptchaBox.Text = captcha;
        }


        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.ViewProductsPage());
        }

        private void ShowCaptchaFields()
        {
            LabelCaptcha.Visibility = Visibility.Visible;
            CaptchaBox.Visibility = Visibility.Visible;
            CaptchaWriteBox.Visibility = Visibility.Visible;
        }

    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GymManagementApplication.Models;

namespace GymManagementApplication
{
    public partial class LoginWindow : Window
    {
        private bool isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
            txtPassword.PasswordChanged += TxtPassword_PasswordChanged;
            txtPasswordVisible.TextChanged += TxtPasswordVisible_TextChanged;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();

        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                txtPassword.Focus();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login();
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
                txtPasswordVisible.Text = txtPassword.Password;
        }

        private void TxtPasswordVisible_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (isPasswordVisible)
                txtPassword.Password = txtPasswordVisible.Text;
        }

        private void btnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            if (isPasswordVisible)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                imgTogglePassword.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/Resources/eye-off.png"));
                txtPasswordVisible.Focus();
                txtPasswordVisible.SelectionStart = txtPasswordVisible.Text.Length;
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                imgTogglePassword.Source = new System.Windows.Media.Imaging.BitmapImage(
                new Uri("pack://application:,,,/Resources/eye.png"));
                txtPassword.Focus();
            }
        }

        private void Login()
        {

            string username = txtUsername.Text.Trim();
            string password = isPasswordVisible ? txtPasswordVisible.Text.Trim() : txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return;
            }

            try
            {
                using (var db = new GymManagementContext())
                {
                    var user = db.Users
                        .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                    if (user != null)
                    {
                        // Đăng nhập thành công, báo cho App.xaml.cs biết
                        this.Tag = user; // Lưu user để truyền sang MainWindow nếu cần
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        lblMessage.Text = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Lỗi kết nối dữ liệu: " + ex.Message;
            }
        }

        private void BtnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Vui lòng liên hệ quản trị viên để lấy lại mật khẩu.", "Quên mật khẩu", MessageBoxButton.OK, MessageBoxImage.Information);
        }


    }
}

using System;
using System.Linq;
using System.Windows;
using GymManagementApplication.Models;

namespace GymManagementApplication
{
    public partial class EditProfileWindow : Window
    {
        private User currentUser;

        public EditProfileWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            UsernameTextBox.Text = currentUser.Username;
            FullNameTextBox.Text = currentUser.FullName;
            RoleTextBox.Text = currentUser.Role;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = CurrentPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            using var db = new GymManagementContext();
            var userInDb = db.Users.FirstOrDefault(u => u.UserId == currentUser.UserId);

            if (userInDb == null)
            {
                MessageBox.Show("Người dùng không tồn tại.");
                return;
            }

            // Kiểm tra mật khẩu hiện tại
            if (userInDb.PasswordHash != currentPassword)
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng.");
                return;
            }

            // Đổi mật khẩu nếu người dùng nhập mới
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu mới không khớp.");
                    return;
                }

                userInDb.PasswordHash = newPassword;
            }

            // Cập nhật họ tên
            userInDb.FullName = FullNameTextBox.Text;

            db.SaveChanges();
            MessageBox.Show("Cập nhật thành công.");
            this.Close();
        }
    }
}

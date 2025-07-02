using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GymManagementApplication.Models;

namespace GymManagementApplication
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private ObservableCollection<Member> _members;

        public MainWindow(User currentUser)
        {
            try
            {
                InitializeComponent();
                _currentUser = currentUser;
                txtUserFullName.Text = _currentUser.FullName ?? _currentUser.Username;
                LoadMembers();
            } catch(Exception e) {
                MessageBox.Show("Lỗi khởi tạo: " + e.Message);
            }
        }

        // ====== MENU ======
        private void BtnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            var editProfileWindow = new EditProfileWindow(_currentUser);
            editProfileWindow.ShowDialog();
        }

        private void BtnViewMembers_Click(object sender, RoutedEventArgs e)
        {
            var membersWindow = new MembersWindow();
            membersWindow.ShowDialog();
        }

        private void BtnManageExpenses_Click(object sender, RoutedEventArgs e)
        {
            var expensesWindow = new ExpensesWindow();
            expensesWindow.ShowDialog();
        }

        private void BtnManageRevenue_Click(object sender, RoutedEventArgs e)
        {
            var revenueWindow = new RevenueWindow();
            revenueWindow.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            
            var loginWindow = new LoginWindow();    
            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                this.Close();
                LoadMembers();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        // ====== DỮ LIỆU HỘI VIÊN ======
        private void LoadMembers()
        {
            using (var db = new GymManagementContext())
            {
                var members = db.Members
                    .Select(m => new Member
                    {
                        MemberId = m.MemberId,
                        FullName = m.FullName,
                        Gender = m.Gender,
                        BirthDate = m.BirthDate,
                        Phone = m.Phone,
                    })
                    .ToList();

                _members = new ObservableCollection<Member>(members);
                UserDataGrid.ItemsSource = _members;
            }
        }

        // Thêm hội viên mới trực tiếp trên DataGrid
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newMember = new Member();
            _members.Add(newMember);
            UserDataGrid.SelectedItem = newMember;
            UserDataGrid.ScrollIntoView(newMember);
            UserDataGrid.BeginEdit();
        }

        // Sửa hội viên: cho phép sửa trực tiếp trên DataGrid, chỉ cần chọn dòng và sửa
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is Member)
            {
                UserDataGrid.BeginEdit();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hội viên để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Xóa hội viên
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is Member selectedMember)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa hội viên {selectedMember.FullName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new GymManagementContext())
                    {
                        var member = db.Members.FirstOrDefault(m => m.MemberId == selectedMember.MemberId);
                        if (member != null)
                        {
                            db.Members.Remove(member);
                            db.SaveChanges();
                        }
                    }
                    _members.Remove(selectedMember);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hội viên để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Reset DataGrid về trạng thái ban đầu
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            LoadMembers();
        }

        // Lưu thay đổi khi sửa trực tiếp trên DataGrid
        private void UserDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            { 
                var editedMember = e.Row.Item as Member;
                if (editedMember != null)
                {
                    using (var db = new GymManagementContext())
                    {
                        if(editedMember.MemberId == 0)
                        {
                            //add
                            db.Members.Add(editedMember);
                            db.SaveChanges();
                            LoadMembers();
                        }
                        else
                        { 
                            //sua
                            var member = db.Members.FirstOrDefault(m => m.MemberId == editedMember.MemberId);
                            if(member != null)
                            {
                                member.FullName = editedMember.FullName;
                                member.Gender = editedMember.Gender;
                                member.BirthDate = editedMember.BirthDate;
                                member.Phone = editedMember.Phone;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

        }
    }
}
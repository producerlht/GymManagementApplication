using GymManagementApplication.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GymManagementApplication
{
    public partial class MembersWindow : Window
    {
        private ObservableCollection<Member> members = new();
        private ObservableCollection<MembershipType> packages = new();
        private Member? selectedMember;

        public MembersWindow()
        {
            InitializeComponent();
            LoadMembers();
            LoadMembershipTypes();
            SetDefaultAd();
        }

        private void LoadMembers()
        {
            using var db = new GymManagementContext();
            var list = db.Members
                .Include(m => m.MemberSubscriptions)
                    .ThenInclude(s => s.Type)
                .ToList();
            members = new ObservableCollection<Member>(list);
            MembersDataGrid.ItemsSource = members;
        }

        private void LoadMembershipTypes()
        {
            using var db = new GymManagementContext();
            packages = new ObservableCollection<MembershipType>(db.MembershipTypes.ToList());
            cbPackageType.ItemsSource = packages;
        }

        private void MembersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMember = MembersDataGrid.SelectedItem as Member;
            if (selectedMember == null)
            {
                ClearDetailPanel();
                return;
            }

            // Thông tin cá nhân
            txtFullName.Text = selectedMember.FullName ?? "";
            txtPhone.Text = selectedMember.Phone ?? "";
            txtAddress.Text = selectedMember.Address ?? "";

            // Gói tập mới nhất (hoặc đang hiệu lực)
            var sub = selectedMember.MemberSubscriptions?
                .OrderByDescending(s => s.IsActive == true)
                .ThenByDescending(s => s.StartDate)
                .FirstOrDefault();

            if (sub != null && sub.Type != null)
            {
                txtPackageName.Text = sub.Type.TypeName;
                txtPackagePrice.Text = $"{sub.Type.Price:N0} đ";
                txtStartDate.Text = sub.StartDate.ToString("dd/MM/yyyy");
                txtEndDate.Text = sub.EndDate.ToString("dd/MM/yyyy");
                txtIsActive.Text = sub.IsActive == true ? "Có" : "Không";
            }
            else
            {
                txtPackageName.Text = "(Chưa đăng ký)";
                txtPackagePrice.Text = txtStartDate.Text = txtEndDate.Text = txtIsActive.Text = "";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newMember = new Member
            {
                FullName = "Họ tên mới",
                Phone = "",
                Address = ""
            };
            members.Add(newMember);
            MembersDataGrid.SelectedItem = newMember;
            MembersDataGrid.ScrollIntoView(newMember);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            MembersDataGrid.IsReadOnly = false;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMember == null)
            {
                MessageBox.Show("Vui lòng chọn hội viên để xóa.");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa hội viên này?", "Xác nhận", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using var db = new GymManagementContext();
                var mem = db.Members.Include(m => m.MemberSubscriptions).FirstOrDefault(m => m.MemberId == selectedMember.MemberId);
                if (mem != null)
                {
                    db.MemberSubscriptions.RemoveRange(mem.MemberSubscriptions);
                    db.Members.Remove(mem);
                    db.SaveChanges();
                }
                members.Remove(selectedMember);
                selectedMember = null;
                ClearDetailPanel();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using var db = new GymManagementContext();
            foreach (var mem in members)
            {
                if (mem.MemberId == 0)
                {
                    db.Members.Add(mem);
                }
                else
                {
                    db.Members.Update(mem);
                }
            }
            db.SaveChanges();
            LoadMembers();
            MessageBox.Show("Dữ liệu đã được lưu.");
        }

        private void BtnAssignPackage_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMember == null)
            {
                MessageBox.Show("Vui lòng chọn hội viên để gán gói tập.");
                return;
            }

            if (cbPackageType.SelectedItem is not MembershipType selectedType ||
                !dpStartDate.SelectedDate.HasValue || !dpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin gói tập.");
                return;
            }

            var newSub = new MemberSubscription
            {
                MemberId = selectedMember.MemberId,
                TypeId = selectedType.TypeId,
                StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value),
                EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value),
                IsActive = true
            };

            using var db = new GymManagementContext();
            db.MemberSubscriptions.Add(newSub);
            db.SaveChanges();
            // Reload lại thông tin chi tiết
            MembersDataGrid_SelectionChanged(null, null);
            MessageBox.Show("Đã thêm gói tập cho hội viên.");
        }

        private void MembersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }

        private void ClearDetailPanel()
        {
            txtFullName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtPackageName.Text = "";
            txtPackagePrice.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtIsActive.Text = "";
        }

        private void SetDefaultAd()
        {
            // Đặt ảnh quảng cáo mặc định nếu có
            try
            {
                imgAd.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/your_ad_image.png"));
            }
            catch { /* Nếu không có ảnh thì bỏ qua */ }
            txtAdTitle.Text = "Ưu đãi tháng này: Giảm 20% cho hội viên mới!";
            txtAdContent.Text = "Đăng ký ngay để nhận ưu đãi và nhiều phần quà hấp dẫn khác.";
        }
    }
}
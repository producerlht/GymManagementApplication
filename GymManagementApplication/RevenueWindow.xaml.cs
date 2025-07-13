using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GymManagementApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApplication
{
    public partial class RevenueWindow : Window
    {
        private ObservableCollection<Revenue> allRevenues = new();
        private ObservableCollection<Revenue> filteredRevenues = new();
        private Revenue? selectedRevenue;

        public RevenueWindow()
        {
            InitializeComponent();
            LoadRevenues();
        }

        private void LoadRevenues()
        {
            using var db = new GymManagementContext();
            allRevenues = new ObservableCollection<Revenue>(
                db.Revenues.Include(r => r.Member).ToList()
            );
            filteredRevenues = new ObservableCollection<Revenue>(allRevenues);
            RevenueDataGrid.ItemsSource = filteredRevenues;
            RevenueDataGrid.IsReadOnly = true;
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = filteredRevenues.Sum(r => r.Amount);
            TotalAmountTextBlock.Text = $"Tổng doanh thu: {total:N0} VND";
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? from = StartDatePicker.SelectedDate;
            DateTime? to = EndDatePicker.SelectedDate;

            if (from == null || to == null)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và kết thúc.");
                return;
            }

            filteredRevenues = new ObservableCollection<Revenue>(
                allRevenues.Where(r =>
                    r.PaymentDate.ToDateTime(TimeOnly.MinValue) >= from &&
                    r.PaymentDate.ToDateTime(TimeOnly.MinValue) <= to
                )
            );

            RevenueDataGrid.ItemsSource = filteredRevenues;
            UpdateTotal();
        }

        private void RevenueDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRevenue = RevenueDataGrid.SelectedItem as Revenue;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using var db = new GymManagementContext();
            var member = db.Members.FirstOrDefault();
            if (member == null)
            {
                MessageBox.Show("Không có hội viên trong hệ thống.");
                return;
            }

            var newRev = new Revenue
            {
                MemberId = member.MemberId,
                Member = member,
                PaymentDate = DateOnly.FromDateTime(DateTime.Today),
                Amount = 0,
                Note = ""
            };

            allRevenues.Add(newRev);
            filteredRevenues.Add(newRev);
            RevenueDataGrid.SelectedItem = newRev;
            RevenueDataGrid.ScrollIntoView(newRev);
            RevenueDataGrid.IsReadOnly = false;
            UpdateTotal();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRevenue == null)
            {
                MessageBox.Show("Vui lòng chọn bản ghi để sửa.");
                return;
            }

            RevenueDataGrid.IsReadOnly = false;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRevenue == null)
            {
                MessageBox.Show("Vui lòng chọn doanh thu để xóa.");
                return;
            }

            var confirm = MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                using var db = new GymManagementContext();
                var entity = db.Revenues.FirstOrDefault(r => r.RevenueId == selectedRevenue.RevenueId);
                if (entity != null)
                {
                    db.Revenues.Remove(entity);
                    db.SaveChanges();
                }

                allRevenues.Remove(selectedRevenue);
                filteredRevenues.Remove(selectedRevenue);
                selectedRevenue = null;
                UpdateTotal();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using var db = new GymManagementContext();

            foreach (var r in allRevenues)
            {
                if (r.Amount < 0)
                {
                    MessageBox.Show("Số tiền không được âm.");
                    return;
                }

                if (r.RevenueId == 0)
                    db.Revenues.Add(r);
                else
                    db.Revenues.Update(r);
            }

            db.SaveChanges();
            LoadRevenues();
            MessageBox.Show("Lưu thành công.");
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            LoadRevenues();
        }

        private void RevenueDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Optional xử lý khi sửa ô
        }
    }
}

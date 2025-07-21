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
            InitializeFilters();
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

        private void InitializeFilters()
        {
            // Tháng: 1–12
            MonthComboBox.ItemsSource = Enumerable.Range(1, 12);
            MonthComboBox.SelectedIndex = DateTime.Now.Month - 1;

            // Năm: từ 2020 đến hiện tại
            int currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(2020, currentYear - 2020 + 1).ToList();
            YearComboBox.ItemsSource = years;
            YearForMonthComboBox.ItemsSource = years;
            YearComboBox.SelectedItem = currentYear;
            YearForMonthComboBox.SelectedItem = currentYear;
        }

        private void BtnFilterByDate_Click(object sender, RoutedEventArgs e)
        {
            if (FilterDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày cần lọc.");
                return;
            }

            var selectedDate = DateOnly.FromDateTime(FilterDatePicker.SelectedDate.Value);

            var filtered = allRevenues.Where(r => r.PaymentDate == selectedDate).ToList();

            filteredRevenues = new ObservableCollection<Revenue>(filtered);

            RevenueDataGrid.ItemsSource = filteredRevenues;
            decimal total = filteredRevenues.Sum(r => r.Amount);
            TotalAmountTextBlock.Text = $"Tổng doanh thu ngày {selectedDate}: {total:N0} VND";
        }

        private void BtnFilterByMonth_Click(object sender, RoutedEventArgs e)
        {
            if (MonthComboBox.SelectedItem == null || YearForMonthComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn tháng và năm.");
                return;
            }

            int month = (int)MonthComboBox.SelectedItem;
            int year = (int)YearForMonthComboBox.SelectedItem;

            var filtered = allRevenues.Where(r => r.PaymentDate.Month == month && r.PaymentDate.Year == year).ToList();

            filteredRevenues = new ObservableCollection<Revenue>(filtered);
            RevenueDataGrid.ItemsSource = filteredRevenues;

            decimal total = filteredRevenues.Sum(r => r.Amount);
            TotalAmountTextBlock.Text = $"Tổng doanh thu tháng {month}/{year}: {total:N0} VND";
        }

        private void BtnFilterByYear_Click(object sender, RoutedEventArgs e)
        {
            if(YearComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn năm.");
                return;
            }
            int year = (int)YearComboBox.SelectedItem;

            var filtered = allRevenues.Where(r => r.PaymentDate.Year == year).ToList();
            filteredRevenues = new ObservableCollection<Revenue>(filtered);
            RevenueDataGrid.ItemsSource = filteredRevenues;

            decimal total = filteredRevenues.Sum(r => r.Amount);
            TotalAmountTextBlock.Text = $"Tổng doanh thu năm {year}: {total:N0} VND";
        
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

            var newRevenue = new Revenue
            {
                MemberId = member.MemberId,
                Member = member,
                PaymentDate = DateOnly.FromDateTime(DateTime.Today),
                Amount = 0,
                Note = ""
            };

            allRevenues.Add(newRevenue);
            filteredRevenues.Add(newRevenue);
            RevenueDataGrid.SelectedItem = newRevenue;
            RevenueDataGrid.ScrollIntoView(newRevenue);
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
            FilterDatePicker.SelectedDate = null;
            MonthComboBox.SelectedItem = null;
            YearForMonthComboBox.SelectedItem = null;
            YearComboBox.SelectedItem = null;

            filteredRevenues = new ObservableCollection<Revenue>(allRevenues);
            RevenueDataGrid.ItemsSource = filteredRevenues;
            UpdateTotal();
        }

        private void RevenueDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRevenue = RevenueDataGrid.SelectedItem as Revenue;
        }

        private void RevenueDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Xử lý nếu cần
        }
    }
}

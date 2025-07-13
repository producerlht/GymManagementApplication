using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GymManagementApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace GymManagementApplication
{
    public partial class ExpensesWindow : Window
    {
        private ObservableCollection<Expense> allExpenses = new();
        private ObservableCollection<Expense> filteredExpenses = new();
        private Expense? selectedExpense;

        public ExpensesWindow()
        {
            InitializeComponent();
            LoadExpenses();
        }

        private void LoadExpenses()
        {
            using var db = new GymManagementContext();
            allExpenses = new ObservableCollection<Expense>(db.Expenses.ToList());
            filteredExpenses = new ObservableCollection<Expense>(allExpenses);
            ExpensesDataGrid.ItemsSource = filteredExpenses;
            ExpensesDataGrid.IsReadOnly = true;
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            decimal total = (decimal)filteredExpenses.Sum(e => e.Amount);
            TotalAmountTextBlock.Text = $"Tổng chi phí: {total:N0} VND";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchTextBox.Text.Trim().ToLower();
            filteredExpenses = new ObservableCollection<Expense>(
                allExpenses.Where(x => x.ExpenseName != null && x.ExpenseName.ToLower().Contains(keyword))
            );
            ExpensesDataGrid.ItemsSource = filteredExpenses;
            UpdateTotalAmount();
        }

        private void ExpensesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedExpense = ExpensesDataGrid.SelectedItem as Expense;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newExpense = new Expense
            {
                ExpenseName = "Tên chi phí",
                Amount = 0,
                ExpenseDate = DateOnly.FromDateTime(DateTime.Today),
                Note = ""
            };
            allExpenses.Add(newExpense);
            filteredExpenses.Add(newExpense);
            ExpensesDataGrid.SelectedItem = newExpense;
            ExpensesDataGrid.ScrollIntoView(newExpense);
            ExpensesDataGrid.IsReadOnly = false;
            UpdateTotalAmount();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedExpense == null)
            {
                MessageBox.Show("Vui lòng chọn chi phí để sửa.");
                return;
            }
            ExpensesDataGrid.IsReadOnly = false;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedExpense == null)
            {
                MessageBox.Show("Vui lòng chọn chi phí để xóa.");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa chi phí này?", "Xác nhận", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using var db = new GymManagementContext();
                var exp = db.Expenses.FirstOrDefault(x => x.ExpenseId == selectedExpense.ExpenseId);
                if (exp != null)
                {
                    db.Expenses.Remove(exp);
                    db.SaveChanges();
                }
                allExpenses.Remove(selectedExpense);
                filteredExpenses.Remove(selectedExpense);
                selectedExpense = null;
                UpdateTotalAmount();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using var db = new GymManagementContext();

            foreach (var exp in allExpenses)
            {
                if (string.IsNullOrWhiteSpace(exp.ExpenseName))
                {
                    MessageBox.Show("Tên chi phí không được để trống.");
                    return;
                }

                if (exp.Amount < 0)
                {
                    MessageBox.Show("Số tiền không được âm.");
                    return;
                }

                if (exp.ExpenseId == 0)
                    db.Expenses.Add(exp);
                else
                    db.Expenses.Update(exp);
            }

            db.SaveChanges();
            LoadExpenses();
            MessageBox.Show("Dữ liệu đã được lưu.");
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            LoadExpenses();
            SearchTextBox.Text = "";
        }

        private void ExpensesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Nếu cần xử lý sau khi sửa, bạn có thể thêm ở đây
        }
    }
}

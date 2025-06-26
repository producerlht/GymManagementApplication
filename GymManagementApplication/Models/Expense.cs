using System;
using System.Collections.Generic;

namespace GymManagementApplication.Models;

public partial class Expense
{
    public int ExpenseId { get; set; }

    public string? ExpenseName { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? ExpenseDate { get; set; }

    public string? Note { get; set; }
}

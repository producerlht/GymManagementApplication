using System;
using System.Collections.Generic;

namespace GymManagementApplication.Models;

public partial class Revenue
{
    public int RevenueId { get; set; }

    public int MemberId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public virtual Member Member { get; set; } = null!;
}

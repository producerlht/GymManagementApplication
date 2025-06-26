using System;
using System.Collections.Generic;

namespace GymManagementApplication.Models;

public partial class MemberSubscription
{
    public int SubscriptionId { get; set; }

    public int MemberId { get; set; }

    public int TypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual MembershipType Type { get; set; } = null!;
}

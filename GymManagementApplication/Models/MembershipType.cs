using System;
using System.Collections.Generic;

namespace GymManagementApplication.Models;

public partial class MembershipType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationInDays { get; set; }

    public virtual ICollection<MemberSubscription> MemberSubscriptions { get; set; } = new List<MemberSubscription>();
}

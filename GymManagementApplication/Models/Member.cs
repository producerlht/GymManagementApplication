using System;
using System.Collections.Generic;

namespace GymManagementApplication.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateOnly? JoinDate { get; set; }

    public string? MembershipStatus { get; set; }

    public virtual ICollection<MemberSubscription> MemberSubscriptions { get; set; } = new List<MemberSubscription>();

    public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();
}

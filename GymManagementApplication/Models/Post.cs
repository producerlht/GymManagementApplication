using System;
using System.Collections.Generic;

namespace GymManagementApplication.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? ImageUrl { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}

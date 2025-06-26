using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApplication.Models;

public partial class GymManagementContext : DbContext
{
    public GymManagementContext()
    {
    }

    public GymManagementContext(DbContextOptions<GymManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberSubscription> MemberSubscriptions { get; set; }

    public virtual DbSet<MembershipType> MembershipTypes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Revenue> Revenues { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=GymManagement;UId=sa;pwd=123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__Expenses__1445CFF36CB599D0");

            entity.Property(e => e.ExpenseId).HasColumnName("ExpenseID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ExpenseName).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(255);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Members__0CF04B382F8A8B61");

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MembershipStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<MemberSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__MemberSu__9A2B24BD54995CAB");

            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberSubscriptions)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MemberSub__Membe__4316F928");

            entity.HasOne(d => d.Type).WithMany(p => p.MemberSubscriptions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MemberSub__TypeI__440B1D61");
        });

        modelBuilder.Entity<MembershipType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Membersh__516F0395A878B886");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126038A7FC0AC1");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__CreatedBy__47DBAE45");
        });

        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.HasKey(e => e.RevenueId).HasName("PK__Revenue__275F173DF3A92F2B");

            entity.ToTable("Revenue");

            entity.Property(e => e.RevenueId).HasColumnName("RevenueID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.Member).WithMany(p => p.Revenues)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Revenue__MemberI__4AB81AF0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACF5618803");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E41C2202C0").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

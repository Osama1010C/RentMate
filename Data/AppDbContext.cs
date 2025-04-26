using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RentMateAPI.Data.Models;

namespace RentMateAPI.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<PendingLandlord> PendingLandlords { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyImage> PropertyImages { get; set; }

    public virtual DbSet<PropertyView> PropertyViews { get; set; }

    public virtual DbSet<RentalRequest> RentalRequests { get; set; }

    public virtual DbSet<SavedPost> SavedPosts { get; set; }

    public virtual DbSet<TenantProperty> TenantProperties { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server = DESKTOP-TI3609O\\SQLEXPRESS ; Database = RentMate ; Integrated Security = SSPI ; TrustServerCertificate = True");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=SQL6032.site4now.net;Initial Catalog=db_ab8220_rentmatedbproject0;User Id=db_ab8220_rentmatedbproject0_admin;Password=zN9ksQSfN#5RP.m");




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3214EC075CAD5CD1");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Property).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__Comments__Proper__72C60C4A");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__UserId__71D1E811");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC07703CDAE4");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__Messages__Receiv__6E01572D");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Sender__6D0D32F4");
        });

        modelBuilder.Entity<PendingLandlord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PendingL__3214EC07D10F6403");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Image).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Name).HasMaxLength(40);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Properti__3214EC077C8E47B5");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PropertyApproval)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("available");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Views).HasDefaultValue(0);

            entity.HasOne(d => d.Landlord).WithMany(p => p.Properties)
                .HasForeignKey(d => d.LandlordId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Propertie__Landl__5535A963");
        });

        modelBuilder.Entity<PropertyImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Property__3214EC07DDD21280");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyImages)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__PropertyI__Prope__5BE2A6F2");
        });

        modelBuilder.Entity<PropertyView>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Property__3214EC070D2ED0B1");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyViews)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__PropertyV__Prope__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.PropertyViews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PropertyV__UserI__5812160E");
        });

        modelBuilder.Entity<RentalRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RentalRe__3214EC074127494F");

            entity.ToTable("RentalRequest");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Property).WithMany(p => p.RentalRequests)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__RentalReq__Prope__619B8048");

            entity.HasOne(d => d.Tenant).WithMany(p => p.RentalRequests)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK__RentalReq__Tenan__60A75C0F");
        });

        modelBuilder.Entity<SavedPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SavedPos__3214EC07C08CF2FE");

            entity.HasOne(d => d.Property).WithMany(p => p.SavedPosts)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__SavedPost__Prope__693CA210");

            entity.HasOne(d => d.Tenant).WithMany(p => p.SavedPosts)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK__SavedPost__Tenan__68487DD7");
        });

        modelBuilder.Entity<TenantProperty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TenantPr__3214EC07DFE152C4");

            entity.HasOne(d => d.Property).WithMany(p => p.TenantProperties)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK__TenantPro__Prope__656C112C");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TenantProperties)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK__TenantPro__Tenan__6477ECF3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C2651A8D");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Image).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Name).HasMaxLength(40);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

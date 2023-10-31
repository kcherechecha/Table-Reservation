using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebRestik.Models;

public partial class RestaurantWebContext : DbContext
{
    public RestaurantWebContext()
    {
    }

    public RestaurantWebContext(DbContextOptions<RestaurantWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<Waiter> Waiters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-CFOLTDF\\SQLEXPRESS;Database=RestaurantWeb;Trusted_Connection=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menu");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.DishName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Photo)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingTime).HasColumnType("datetime");
            entity.Property(e => e.ClientName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Table).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Table");

            entity.HasOne(d => d.Waiter).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.WaiterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Waiter");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("Restaurant");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Longtitude).HasColumnName("longtitude");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.ToTable("Table");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Number)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Tables)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_Restaurant");
        });

        modelBuilder.Entity<Waiter>(entity =>
        {
            entity.ToTable("Waiter");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Waiters)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Waiter_Restaurant");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

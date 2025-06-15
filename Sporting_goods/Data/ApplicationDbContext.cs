using Microsoft.EntityFrameworkCore;
using Sporting_goods.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sporting_goods.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Order1> Orders { get; set; }
        public DbSet<PickupPoint> PickupPoints { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-KEHORP4;Database=Спортивные_товары;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.Property(p => p.ProductPhoto)
                      .HasColumnName("ProductPhoto")
                      .HasColumnType("nvarchar(500)")
                      .IsRequired(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(op => new { op.OrderID, op.ProductArticleNumber });
            });

            modelBuilder.Entity<PickupPoint>(entity =>
            {
                entity.ToTable("PickupPoint");
                entity.Property(e => e.IDPick_upPoint)
                      .HasColumnName("IDPick-upPoint");
                entity.Property(e => e.Index)
                      .HasColumnName("Index");
                entity.Property(e => e.Address)
                      .HasColumnName("Address")
                      .HasColumnType("nvarchar(100)")
                      .IsRequired(false);
            });

            modelBuilder.Entity<Order1>(entity =>
            {
                entity.ToTable("Order1");
                entity.Property(e => e.OrderID)
                      .HasColumnName("OrderID");
                entity.Property(e => e.OrderStatus)
                      .HasColumnName("OrderStatus");
                entity.Property(e => e.OrderDate)
                      .HasColumnName("OrderDate");
                entity.Property(e => e.OrderDeliveryDate)
                      .HasColumnName("OrderDeliveryDate");
                entity.Property(e => e.OrderPickupPoint)
                      .HasColumnName("OrderPickupPoint");
            });

            modelBuilder.Entity<Order1>()
                .HasOne(o => o.PickupPoint)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.OrderPickupPoint)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductArticleNumber)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.UserRole)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
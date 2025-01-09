using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Models;

namespace MvcLaptop.Data
{
    public class MvcLaptopContext : IdentityDbContext<User, IdentityRole, string>
    {
        public MvcLaptopContext(DbContextOptions<MvcLaptopContext> options) : base(options)
        {
        }
        public DbSet<Product> Product { get; set; } = default!;
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Category>? Category { get; set; }
        public DbSet<ProductImage>? ProductImage { get; set; }
        public DbSet<OrderDetail>? OrderDetail { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // if (!modelBuilder.Model.GetEntityTypes().Any(t => t.Name == "Category"))
            // {
            //     modelBuilder.Entity<Category>().ToTable("Category");
            // }
            // if (!modelBuilder.Model.GetEntityTypes().Any(t => t.Name == "Product"))
            // {
            //     modelBuilder.Entity<Product>().ToTable("Product");
            // }if (!modelBuilder.Model.GetEntityTypes().Any(t => t.Name == "ProductImage"))
            // {
            //     modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
            // }
            // Đặt tên bảng
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<ProductImage>()
                .HasKey(pi => pi.Id);

            // Thiết lập quan hệ 1-nhiều giữa Category và Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category) // Product có một Category
                .WithMany(c => c.Products) // Category có nhiều Product
                .HasForeignKey(p => p.CategoryId);

            // Thiết lập quan hệ 1-nhiều giữa Product và ProductImage
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages) //có nhiều ProductImage
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Product sẽ xóa ProductImage liên quan
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .IsRequired();
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName!.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }

    }
}

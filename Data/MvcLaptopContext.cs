using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Models;

namespace MvcLaptop.Data
{
    public class MvcLaptopContext(DbContextOptions<MvcLaptopContext> options) : DbContext(options)
    {
        public DbSet<Product> Product { get; set; } = default!;
        public DbSet<User>? Users { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Category>? Category { get; set; }
        public DbSet<ProductImage>? ProductImage { get; set; }
    }
}

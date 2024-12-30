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
        public DbSet<MvcLaptop.Models.Laptop> Laptop { get; set; } = default!;
        public DbSet<MvcLaptop.Models.User>? Users { get; set; }
        public DbSet<MvcLaptop.Models.Order>? Orders { get; set; }
    }
}

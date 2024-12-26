using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Models;

namespace MvcLaptop.Data
{
    public class MvcLaptopContext : DbContext
    {
        public MvcLaptopContext (DbContextOptions<MvcLaptopContext> options)
            : base(options)
        {
        }

        public DbSet<MvcLaptop.Models.Laptop> Laptop { get; set; } = default!;
        public DbSet<MvcLaptop.Models.User> Users { get; set; }
    }
}

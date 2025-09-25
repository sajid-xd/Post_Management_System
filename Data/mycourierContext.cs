using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mycourier.Models;

namespace mycourier.Data
{
    public class mycourierContext : DbContext
    {
        public mycourierContext(DbContextOptions<mycourierContext> options)
            : base(options)
        {
        }

        // Existing tables
        public DbSet<Location> Location { get; set; } = default!;
        public DbSet<User> User { get; set; } = default!;

        // ✅ Added tables
        public DbSet<Service> Service { get; set; } = default!;
        public DbSet<Weight> Weight { get; set; } = default!;
        public DbSet<Delivery> Delivery { get; set; } = default!;
    }
}

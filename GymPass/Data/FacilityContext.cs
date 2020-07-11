using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GymPass.Models;

namespace GymPass.Data
{
    public class FacilityContext : DbContext
    {
        public FacilityContext(DbContextOptions<FacilityContext> options)
            : base(options)
        {
        }

        public DbSet<Facility> Facilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Facility>().ToTable("Facility");
        }
    }
}

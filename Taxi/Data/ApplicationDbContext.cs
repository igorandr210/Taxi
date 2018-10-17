using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Entities;

namespace Taxi.Data
{
    public class ApplicationDbContext: IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions options): base (options)
        {
         
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql()

        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.Entity<Contract>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
            base.OnModelCreating(modelBuilder);
            
            
        }

        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Trip> Trips { get; set; }
        
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Picture> Pictures { get; set; }

        public DbSet<ProfilePicture> ProfilePictures { get; set; }

        public DbSet<TripHistory> TripHistories { get; set; }
        
        public DbSet<TripRouteNode> TripRouteNodes { get; set; }

        public DbSet<DriverLicense> DriverLicenses { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<TripHistoryRouteNode> TripHistoryRouteNodes { get; set; }

        public DbSet<RefundRequest> RefundRequests { get; set; }

        public DbSet<AdminResponse> AdminResponces { get; set; }
    }
}

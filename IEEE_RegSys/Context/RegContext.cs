using IEEE_RegSys.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace IEEE_RegSys.Context
{
    public class RegContext : DbContext
    {
        public RegContext(DbContextOptions<RegContext> options) : base(options) { }


        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<PromoCode> PromoCodes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed an admin user (password hashed as plain for demo - replace with real hashing)
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "admin123", // change in production
                Role = "Admin"
            });
        }
    }
}

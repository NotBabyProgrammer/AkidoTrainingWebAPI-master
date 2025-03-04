using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AkidoTrainingWebAPI.DataAccess.Models;

namespace AkidoTrainingWebAPI.DataAccess.Data
{
    public class AkidoTrainingWebAPIContext : DbContext
    {
        public AkidoTrainingWebAPIContext(DbContextOptions<AkidoTrainingWebAPIContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for Accounts
            modelBuilder.Entity<Accounts>().HasData(
                new Accounts
                {
                    Id = 1,
                    Name = "Admin",
                    PhoneNumber = "11111",
                    Password = "Password123",
                    Role = "Admin",
                    Belt = "Black",
                    Level = 0,
                    ImagePath = "Default.png"
                },
                new Accounts
                {
                    Id = 2,
                    Name = "User1",
                    PhoneNumber = "22222",
                    Password = "Password123",
                    Role = "User",
                    Belt = "Black",
                    Level = 0,
                    ImagePath = "Default.png"
                },
                new Accounts
                {
                    Id = 3,
                    Name = "User2",
                    PhoneNumber = "33333",
                    Password = "Password123",
                    Role = "User",
                    Belt = "Black",
                    Level = 0,
                    ImagePath = "Default.png"
                }
            );

            modelBuilder.Entity<Roles>().HasData(
                new Roles
                {
                    RolesId = 1,
                    RoleName = "Admin"
                },
                new Roles
                {
                    RolesId = 2,
                    RoleName = "User"
                }
                );
        }

        public DbSet<Accounts> Accounts { get; set; } = default!;
        public DbSet<Roles> Roles { get; set; } = default!;
        public DbSet<AkidoTrainingWebAPI.DataAccess.Models.Posts> Posts { get; set; } = default!;
        public DbSet<AkidoTrainingWebAPI.DataAccess.Models.Contents> Contents { get; set; } = default!;
        public DbSet<AkidoTrainingWebAPI.DataAccess.Models.Districts> Districts { get; set; } = default!;
        public DbSet<AkidoTrainingWebAPI.DataAccess.Models.Areas> Areas { get; set; } = default!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DinoRimas.Models;
using System.Text.Json;

namespace DinoRimas.Data
{
    public class DinoRimasDbContext : DbContext
    {
        public DinoRimasDbContext(DbContextOptions options) :base(options){}
        public DbSet<UserModel> Users { get; set; }
        public DbSet<DinoModel> DinoModels { get; set; }
        public DbSet<UnitPayLogsModel> UnitPayLogs { get; set; }
        public DbSet<DonateShopLogsModel> DonateShopLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Inventory);

            modelBuilder.Entity<DinoModel>()
                .Ignore(d => d.Name);

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DinoRimas.Models;
using Newtonsoft.Json;

namespace DinoRimas.Data
{
    public class DinoRimasDbContext : DbContext
    {
        public DinoRimasDbContext(DbContextOptions options) :base(options){}
        public DbSet<UserModel> User { get; set; }
        //public DbSet<DinoModel> Dinos { get; set; }
        public DbSet<DinoShopModel> ShopDinoList { get; set; }
        public DbSet<UnitPayLogsModel> UnitPayLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DinoModel>()
                .Property(d => d.Config)
                .HasConversion(
                    s => JsonConvert.SerializeObject(s),
                    s => JsonConvert.DeserializeObject<DinoSaveModel>(s)
                );                       

            modelBuilder.Entity<DinoShopModel>()
               .Property(d => d.BaseConfig)
               .HasConversion(
                   s => JsonConvert.SerializeObject(s),
                   s => JsonConvert.DeserializeObject<DinoSaveModel>(s)
               );
        }

    }
}

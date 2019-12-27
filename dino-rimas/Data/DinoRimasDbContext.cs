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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
           optionsBuilder.UseNpgsql(Program.Settings.ConnectionString);
        }

        public DbSet<UserModel> User { get; set; }
        public DbSet<DinoModel> Dinos { get; set; }
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

            modelBuilder.Entity<UserModel>()
                .HasMany(u=>u.Inventory)
                .WithOne(d => d.Owner); 

            modelBuilder.Entity<UserModel>()
                .Property(u=>u.BaseDinos)
                .HasConversion(
                   s => JsonConvert.SerializeObject(s),
                   s => JsonConvert.DeserializeObject<List<DinoModel>>(s)
               );
            //DataSeeder(modelBuilder);
        }

        private void DataSeeder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DinoShopModel>()
                .HasData(
                    new DinoShopModel { 
                        Id = 1, 
                        Name = "Читазавр", 
                        Img = "TestImg.png", 
                        Description = "Особый вид травоядного который любит самоутверждаться за счет использования запрещенного ПО", 
                        Price = 1, 
                        Survival = false 
                    },
                    new DinoShopModel { Id = 2, Name = "Прогерозавр", Img = "TestImg.png", Description = "Этот вид относится к семейству травоядных, отличительной чертой которгог является квартирный образ жизни и провождение времени за компьютером", Price = 10000, Survival = true },
                    new DinoShopModel { Id = 3, Name = "Игромазавр", Img = "TestImg.png", Description = "Он как и особь вида прогерозавр семейства травоядных много времени проводит за компом, но в отличии от первого приносит мало пользы", Price = 1000, Survival = false },
                    new DinoShopModel { Id = 4, Name = "Тролезавр", Img = "TestImg.png", Description = "Гинетичесская модификация вида геймер заставила этот вид стать на путь плотоядных, громкий рык но в тоже время относительная беспомощность сделала его падальщиком", Price = 2, Survival = false },
                    new DinoShopModel { Id = 5, Name = "Админозавр", Img = "TestImg.png", Description = "Один из сильнейших вид хищников одаренный природой громадной силой, является вожаком и несет ответственность за свою стаю", Price = 9000, Survival = true },
                    new DinoShopModel { Id = 6, Name = "Модерозавр", Img = "TestImg.png", Description = "Особь более низкого уровня чем админозавр. Обычно обитают небольшими группами в стае Админозавр и регулируют в ней порядок путем применения грубой силы" , Price = 7000, Survival = true },
                    new DinoShopModel { Id = 7, Name = "Стримерозавр", Img = "TestImg.png", Description = "Особый вид травоядного который любит путешествовать по всему миру и делиться информацией с другими особями. В результате многие виды мигрируют в другие места благодоря его рассказам.", Price = 8000, Survival = false }
                    //new DinoShopModel { Name = "Читак", Img = "TestImg", Description = "Особый вид травоядного который любит самоутверждаться за счет использования запрещенного ПО", Price = 0, Survival = false }
                );
        }

    }
}

using System;
using FakeWebShop.Persistence.Entities.Model;
using Microsoft.EntityFrameworkCore;

namespace FakeWebShop.Persistence;

public class ProductDbContext: DbContext
{
       private string connectionString = "server=localhost;port=3308;database=FakeWebShop;user=root;password=WachtW00rd;"; 
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ClothingProduct> ClothingProducts { get; set; }
        public DbSet<Tshirt> Tshirts { get; set; }
        public DbSet<Hoodie> Hoodies { get; set; }

        public DbSet<DrinkProduct> DrinkProducts { get; set; }
        public DbSet<Mok> Mokken { get; set; }
        public DbSet<Drinkfles> DrinkFlessen { get; set; }

        public DbSet<Pen> Pennen { get; set; }
        public DbSet<NoteBook> NoteBooks { get; set; }
        public DbSet<Sticker> Stickers { get; set; }
        public DbSet<ToteBag> ToteBags { get; set; }
        public DbSet<Powerbank> Powerbanks { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySql(
            connectionString,
            new MySqlServerVersion(ServerVersion.AutoDetect(connectionString))
        );
    }


    


}

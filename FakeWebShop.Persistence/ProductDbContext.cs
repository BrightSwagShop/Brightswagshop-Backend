using System;
using FakeWebShop.Persistence.Entities.Model;
using FakeWebShop.Persistence.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FakeWebShop.Persistence;

public class ProductDbContext(IOptions<ProductRepositoryOptions> options) : DbContext
{

    protected override void OnConfiguring(DbContextOptionsBuilder dbOptions)
    {
        var cs = options.Value.ConnectionString;
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Connection string is missing in ProductRepositoryOptions.");

        dbOptions.UseMySql(cs, new MySqlServerVersion(ServerVersion.AutoDetect(cs)));
    }


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




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TPT mapping
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<ClothingProduct>().ToTable("ClothingProducts");
        modelBuilder.Entity<Tshirt>().ToTable("Tshirts");
        modelBuilder.Entity<Hoodie>().ToTable("Hoodies");
        modelBuilder.Entity<DrinkProduct>().ToTable("DrinkProducts");
        modelBuilder.Entity<Mok>().ToTable("Mokken");
        modelBuilder.Entity<Drinkfles>().ToTable("DrinkFlessen");
        modelBuilder.Entity<Powerbank>().ToTable("Powerbanks");
        modelBuilder.Entity<Pen>().ToTable("Pennen");
        modelBuilder.Entity<Sticker>().ToTable("Stickers");
        modelBuilder.Entity<NoteBook>().ToTable("NoteBooks");
        modelBuilder.Entity<ToteBag>().ToTable("ToteBags");

        // Category (1) -> Products (many)
        modelBuilder.Entity<Product>()
            .HasOne<Category>()               // of .HasOne(p => p.Category) als je nav property hebt
            .WithMany(c => c.Producten)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }





}

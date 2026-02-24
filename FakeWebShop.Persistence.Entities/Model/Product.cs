using System;

namespace FakeWebShop.Persistence.Entities.Model;

public abstract class Product
{
    protected Product(Guid id, string naam, string beschrijving, decimal prijs, string imageUrl, Guid categoryId)
    {
        Id = id;
        Naam = naam;
        Beschrijving = beschrijving;
        Prijs = prijs;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
    }

    public Guid Id { get; set; }
    public string Naam{get; set;}

    public string Beschrijving{get; set;}

    public decimal Prijs{get; set;}
    public string ImageUrl{get; set;}

    public Guid CategoryId{get; set;}
}

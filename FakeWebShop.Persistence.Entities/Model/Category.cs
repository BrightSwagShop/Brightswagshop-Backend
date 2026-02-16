using System;

namespace FakeWebShop.Persistence.Entities.Model;

public class Category
{
    public Category(Guid id, string naam )
    {
        Id = id;
        Naam = naam;
        Producten = new List<Product>();
       
    }
     

    public Guid Id{ get; set; }
    public string Naam{ get; set; }
    public List<Product> Producten { get; set;}


}

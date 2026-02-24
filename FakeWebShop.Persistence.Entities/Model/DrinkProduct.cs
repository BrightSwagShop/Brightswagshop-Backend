using System;

namespace FakeWebShop.Persistence.Entities.Model;

public abstract class DrinkProduct: Product
{
    public DrinkProduct(Guid id, string naam,string beschrijving, decimal prijs, string imageUrl, Guid categoryId, int inhoud, string kleur):base(id,naam, beschrijving, prijs, imageUrl, categoryId)
    {
        Inhoud = inhoud;
        Kleur = kleur;
    }

    public int Inhoud{ get; set; }
    public string Kleur{ get; set; }


}

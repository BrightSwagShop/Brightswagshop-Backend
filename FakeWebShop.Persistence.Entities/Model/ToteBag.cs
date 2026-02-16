using System;

namespace FakeWebShop.Persistence.Entities.Model;

public class ToteBag:Product
{
    public ToteBag (Guid id,
        string naam,
        string beschrijving,
        decimal prijs,
        string imageUrl,
        Guid categoryId,
        double breedte,
        double hoogte, string kleur): base(id, naam, beschrijving, prijs, imageUrl, categoryId)
    {
        Breedte = breedte;
        Hoogte = hoogte;
        Kleur = kleur;
        
    }

    public double Breedte { get; set;}
    public double Hoogte { get; set;}
    public string Kleur{ get; set;}


}

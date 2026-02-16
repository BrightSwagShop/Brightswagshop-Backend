using System;
using System.Security.Cryptography.X509Certificates;

namespace FakeWebShop.Persistence.Entities.Model;

public class Pen: Product
{
    public Pen(Guid id,
        string naam,
        string beschrijving,
        decimal prijs,
        string imageUrl,
        Guid categoryId,
        string kleur): base(id, naam, beschrijving, prijs, imageUrl, categoryId)
    {
     Kleur = kleur;
        
    }

    public string Kleur{ get; set; }

}

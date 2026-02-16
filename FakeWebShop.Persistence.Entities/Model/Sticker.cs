using System;

namespace FakeWebShop.Persistence.Entities.Model;

public class Sticker : Product
{
    public Sticker( Guid id,
        string naam,
        string beschrijving,
        decimal prijs,
        string imageUrl,
        Guid categoryId,
        double breedte,
        double hoogte): base(id, naam, beschrijving, prijs, imageUrl, categoryId)
    {
        Breedte = breedte;
        Hoogte = hoogte;
        
    }
    public double Breedte{ get; set; }
    public double Hoogte{ get; set; }


}

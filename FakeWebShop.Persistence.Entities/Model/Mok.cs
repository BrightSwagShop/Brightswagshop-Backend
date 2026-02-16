using System;

namespace FakeWebShop.Persistence.Entities.Model;

public class Mok : DrinkProduct
{
      public bool VaatwasserBestendig { get; private set; }
   public Mok(
        Guid id,
        string naam,
        string beschrijving,
        decimal prijs,
        string imageUrl,
        Guid categoryId,
        int inhoud,
        string kleur,
        bool vaatwasserBestendig)
        : base(id, naam, beschrijving, prijs, imageUrl, categoryId, inhoud, kleur)
    {
        VaatwasserBestendig = vaatwasserBestendig;
    }



}

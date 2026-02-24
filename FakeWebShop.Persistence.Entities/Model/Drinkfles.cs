using System;
using FakeWebShop.Domain.Model;
 

namespace FakeWebShop.Persistence.Entities.Model;

public class Drinkfles: DrinkProduct
{
    public bool IsThermisch { get;   set; }
    public DrinkFlesMateriaal Materiaal { get;   set; }

    public Drinkfles(
        Guid id,
        string naam,
        string beschrijving,
        decimal prijs,
        string imageUrl,
        Guid categoryId,
        int inhoud,
        string kleur,
        bool isThermisch,
        DrinkFlesMateriaal materiaal)
        : base(id, naam, beschrijving, prijs, imageUrl, categoryId, inhoud, kleur)
    {
        Materiaal = materiaal;
        IsThermisch = isThermisch;
    }

}

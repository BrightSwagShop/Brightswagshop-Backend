using System;
using FakeWebShop.Domain.Model;

namespace FakeWebShop.Contracts;

public class DrinkflesRequestContract
{
    public string Naam { get; set; } = string.Empty;
    public string Beschrijving { get; set; } = string.Empty;
    public decimal Prijs { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    public int Inhoud { get; set; }
    public string Kleur { get; set; } = string.Empty;
    public bool IsThermisch { get; set; }
    public DrinkFlesMateriaal Materiaal { get; set; }

}

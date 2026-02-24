using System;
using FakeWebShop.Domain.Model;

namespace FakeWebShop.Contracts;

public class DrinkflesResponseContract : ProductResponseContract
{
     public int Inhoud { get; set; }

    public string Kleur { get; set; } = string.Empty;

    public bool IsThermisch { get; set; }

    public DrinkFlesMateriaal Materiaal { get; set; }


}

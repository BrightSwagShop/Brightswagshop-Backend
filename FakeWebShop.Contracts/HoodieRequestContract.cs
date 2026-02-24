using System;
using FakeWebShop.Domain.Model;

namespace FakeWebShop.Contracts.Enums;

public class HoodieRequestContract
{
      public string Naam { get; set; } = string.Empty;

    public string Beschrijving { get; set; } = string.Empty;

    public decimal Prijs { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Maat Maat { get; set; }

    public Stof Stof { get; set; }

    public string Kleur { get; set; } = string.Empty;

    public bool HasZipper { get; set; }

    public PocketType PocketType { get; set; } 

}

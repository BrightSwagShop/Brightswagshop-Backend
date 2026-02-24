using System;
using FakeWebShop.Domain.Model;

namespace FakeWebShop.Contracts;

public class HoodieResponseContract :ProductResponseContract
{
    public Maat Maat { get; set; }

    public Stof Stof { get; set; }

    public string Kleur { get; set; } = string.Empty;

    public bool HasZipper { get; set; }

    public PocketType PocketType { get; set; } 

}

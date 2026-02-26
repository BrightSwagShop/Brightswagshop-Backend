using System;
using FakeWebShop.Contracts;
using FakeWebShop.Domain.Model;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Domain.Services.Mapping;

public static class DrinkflesMappingExtensions
{
    // REQUEST → DOMAIN
    public static DrinkflesModel AsModel(this DrinkflesRequestContract contract)
    {
        return new DrinkflesModel
        {
            Naam = contract.Naam,
            Beschrijving = contract.Beschrijving,
            Prijs = contract.Prijs,
            ImageUrl = contract.ImageUrl,
            CategoryId = contract.CategoryId,
            Inhoud = contract.Inhoud,
            Kleur = contract.Kleur,
            IsThermisch = contract.IsThermisch,
            Materiaal = contract.Materiaal
        };
    }

    // DOMAIN → RESPONSE
    public static DrinkflesResponseContract AsContract(this DrinkflesModel model)
    {
        return new DrinkflesResponseContract
        {
            Id = model.Id ?? throw new Exception(),
            Naam = model.Naam,
            Beschrijving = model.Beschrijving,
            Prijs = model.Prijs,
            ImageUrl = model.ImageUrl,
            CategoryId = model.CategoryId,
            Inhoud = model.Inhoud,
            Kleur = model.Kleur,
            IsThermisch = model.IsThermisch,
            Materiaal = model.Materiaal
        };
    }

    // ENTITY → DOMAIN
    public static DrinkflesModel AsModel(this Drinkfles entity)
    {
        return new DrinkflesModel
        {
            Id = entity.Id,
            Naam = entity.Naam,
            Beschrijving = entity.Beschrijving,
            Prijs = entity.Prijs,
            ImageUrl = entity.ImageUrl,
            CategoryId = entity.CategoryId,
            Inhoud = entity.Inhoud,
            Kleur = entity.Kleur,
            IsThermisch = entity.IsThermisch,
            Materiaal = entity.Materiaal
        };
    }

    // DOMAIN → ENTITY
    public static Drinkfles AsEntity(this DrinkflesModel model)
    {
        return new Drinkfles(
            model.Id ?? Guid.NewGuid(),
            model.Naam ?? throw new Exception(),
            model.Beschrijving ?? throw new Exception(),
            model.Prijs,
            model.ImageUrl ?? throw new Exception(),
            model.CategoryId,
            model.Inhoud,
            model.Kleur ?? throw new Exception(),
            model.IsThermisch,
            model.Materiaal
        );
    }

}

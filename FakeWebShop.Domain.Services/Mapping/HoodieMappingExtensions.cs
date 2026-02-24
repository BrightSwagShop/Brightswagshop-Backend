using System;
using FakeWebShop.Contracts;
using FakeWebShop.Contracts.Enums;
using FakeWebShop.Domain.Model;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Domain.Services.Mapping;

public static class HoodieMappingExtensions
{
     public static HoodieModel AsModel(this HoodieRequestContract contract)
    {
        return new HoodieModel
        {
            Naam = contract.Naam,
            Beschrijving = contract.Beschrijving,
            Prijs = contract.Prijs,
            ImageUrl = contract.ImageUrl,
            CategoryId = contract.CategoryId,
            Maat = contract.Maat,
            Stof = contract.Stof,
            Kleur = contract.Kleur,
            HasZipper = contract.HasZipper,
            PocketType = contract.PocketType
        };
    }

    public static HoodieResponseContract AsContract(this HoodieModel model)
    {
        return new HoodieResponseContract
        {
            Id = model.Id ?? throw new  Exception(),
            Naam = model.Naam,
            Beschrijving = model.Beschrijving,
            Prijs = model.Prijs,
            ImageUrl = model.ImageUrl,
            CategoryId = model.CategoryId,
            Maat = model.Maat,
            Stof = model.Stof,
            Kleur = model.Kleur,
            HasZipper = model.HasZipper,
            PocketType = model.PocketType
        };
    }

    public static HoodieModel AsModel(this Hoodie entity)
    {
        var model = new HoodieModel();
        ProductMappingExtensions.MapBaseToModel(entity, model);

        model.Maat = entity.Maat;
        model.Stof = entity.Stof;
        model.Kleur = entity.Kleur;
        model.HasZipper = entity.HasZipper;
        model.PocketType = entity.PocketType;

        return model;
    }

    public static Hoodie AsEntity(this HoodieModel model)
    {
        var entity = new Hoodie(
            model.Id ?? Guid.NewGuid(),
            model.Naam ?? throw new  Exception(),
            model.Beschrijving ?? throw new  Exception(),
            model.Prijs,
            model.ImageUrl ?? throw new  Exception(),
            model.CategoryId,
            model.Maat,
            model.Stof,
            model.Kleur ?? throw new  Exception(),
            model.HasZipper,
            model.PocketType  
        );

        return entity;
    }

}

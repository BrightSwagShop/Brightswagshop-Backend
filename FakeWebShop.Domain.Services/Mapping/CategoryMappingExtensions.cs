using System;
using FakeWebShop.Contracts;
using FakeWebShop.Domain.Model;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Domain.Services.Mapping;

public static class CategoryMappingExtensions
{
     public static CategoryModel AsModel(this CategoryRequestContract contract)
    {
        return new CategoryModel
        {
            Naam = contract.Naam
        };
    }

    public static CategoryResponseContract AsContract(this CategoryModel model)
    {
        return new CategoryResponseContract
        {
            Id = model.Id ?? throw new Exception(),
            Naam = model.Naam
        };
    }

    public static CategoryModel AsModel(this Category entity)
    {
        return new CategoryModel
        {
            Id = entity.Id,
            Naam = entity.Naam
        };
    }

    public static Category AsEntity(this CategoryModel model)
    {
        return new Category(
            model.Id ?? Guid.NewGuid(),
            model.Naam ?? throw new Exception()
        );
    }


}

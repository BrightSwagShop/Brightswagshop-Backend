using System;
using FakeWebShop.Contracts.Request.DiscountRequest;
using FakeWebShop.Contracts.Response.DiscountResponse;
using FakeWebShop.Domain.Model.Discount;
using FakeWebShop.Persistence.Entities.Discount;

namespace FakeWebShop.Domain.Services.ServicesMapping.DiscountMapping;

public static class DiscountMapping
{
    public static DiscountModel AsModel(this DiscountRequest request)
    {
        return new DiscountModel
        {
            Name = request.Name,
            Description = request.Description,
            Percentage = request.Percentage,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            IsActive = request.IsActive,
            Items = request.Items.Select(i => i.AsModel()).ToList()
        };
    }

    public static DiscountItemModel AsModel(this DiscountItemRequest request)
    {
        return new DiscountItemModel
        {
            Code = request.Code,
            ProductId = request.ProductId,
            Percentage = request.Percentage,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            IsActive = request.IsActive
        };
    }

    public static Discount AsEntity(this DiscountModel model)
    {
        return new Discount
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Percentage = model.Percentage,
            StartsAt = model.StartsAt,
            EndsAt = model.EndsAt,
            IsActive = model.IsActive,
            Items = model.Items.Select(i => new DiscountItem
            {
                Code = i.Code,
                Percentage = i.Percentage,
                StartsAt = i.StartsAt,
                EndsAt = i.EndsAt,
                IsActive = i.IsActive
            }).ToList()
        };
    }

    public static DiscountModel AsModel(this Discount entity)
    {
        return new DiscountModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Percentage = entity.Percentage,
            StartsAt = entity.StartsAt,
            EndsAt = entity.EndsAt,
            IsActive = entity.IsActive,
            Items = entity.Items.Select(i => new DiscountItemModel
            {
                Code = i.Code,
                Percentage = i.Percentage,
                StartsAt = i.StartsAt,
                EndsAt = i.EndsAt,
                IsActive = i.IsActive
            }).ToList()
        };
    }

    public static DiscountResponse AsResponse(this DiscountModel model)
    {
        return new DiscountResponse
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Percentage = model.Percentage,
            StartsAt = model.StartsAt.UtcDateTime,
            EndsAt = model.EndsAt?.UtcDateTime,
            IsActive = model.IsActive,
            Items = model.Items.Select(item => item.AsResponse()).ToList()
        };
    }

    public static DiscountItemResponse AsResponse(this DiscountItemModel model)
    {
        return new DiscountItemResponse
        {
            Id = model.Id,
            DiscountId = model.DiscountId,
            Code = model.Code,
            ProductId = model.ProductId,
            Percentage = model.Percentage,
            StartsAt = model.StartsAt.UtcDateTime,
            EndsAt = model.EndsAt?.UtcDateTime,
            IsActive = model.IsActive
        };
    }
}
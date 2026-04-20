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
            Code = request.Code,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            IsActive = request.IsActive,
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
            Code = model.Code,
            StartsAt = model.StartsAt,
            EndsAt = model.EndsAt,
            IsActive = model.IsActive,
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
            Code = entity.Code,
            EndsAt = entity.EndsAt,
            IsActive = entity.IsActive,
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
            Code = model.Code,
            StartsAt = model.StartsAt.UtcDateTime,
            EndsAt = model.EndsAt?.UtcDateTime,
            IsActive = model.IsActive,
        };
    }
}
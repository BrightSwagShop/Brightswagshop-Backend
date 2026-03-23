using System;
using FakeWebShop.Contracts.Request.DiscountRequest;
using FakeWebShop.Contracts.Response.DiscountResponse;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Domain.Model.Discount;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Domain.Services.ServicesMapping.DiscountMapping;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

namespace FakeWebShop.Domain.Services;

public class DiscountService(IDiscountRepository discountRepo) : IDiscountService
{
    public async Task<DiscountResponse> CreateAsync(DiscountRequest request)
    {
        var discountModel = request.AsModel();

        var entity = discountModel.AsEntity();

        await discountRepo.CreateAsync(entity);

        return entity.AsModel().AsResponse();
    }

    public async Task<DiscountResponse?> GetByIdAsync(string id)
    {
        var entity = await discountRepo.GetByIdAsync(id);

        if (entity is null)
            return null;

        return entity.AsModel().AsResponse();
    }

    public async Task<List<DiscountResponse>> GetAllAsync()
    {
        var entities = await discountRepo.GetAllAsync();

        return entities
            .Select(entity => entity.AsModel().AsResponse())
            .ToList();
    }
}
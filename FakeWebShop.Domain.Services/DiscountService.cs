using System;
using FakeWebShop.Contracts.Request.DiscountRequest;
using FakeWebShop.Contracts.Response.DiscountResponse;
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

    public async Task<DiscountResponse?> GetByCodeAsync(string code)
    {
        var entity = await discountRepo.GetByCodeAsync(code);

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

    public async Task<DiscountResponse?> UpdateAsync(string id, DiscountRequest request)
    {
        var existing = await discountRepo.GetByIdAsync(id);
        if (existing is null)
        {
            return null;
        }

        var updated = request.AsModel().AsEntity();
        updated.Id = id;

        await discountRepo.UpdateAsync(updated);

        return updated.AsModel().AsResponse();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var existing = await discountRepo.GetByIdAsync(id);
        if (existing is null)
        {
            return false;
        }

        await discountRepo.DeleteAsync(id);
        return true;
    }
}
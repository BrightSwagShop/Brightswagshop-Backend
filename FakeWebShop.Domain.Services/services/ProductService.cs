using System;
using FakeWebShop.Contracts;
using FakeWebShop.Domain.Services.Interfaces;
using FakeWebShop.Domain.Services.Mapping;
using FakeWebShop.Persistence.repos;

namespace FakeWebShop.Domain.Services.services;

public class ProductService(IProductsRepo repository) : IProductService
{
     public async Task<ProductResponseContract?> GetProductAsync(Guid id)
    {
        var entity = await repository.GetProductAsync(id);
        if (entity == null) return null;

        return entity.AsContract(); // ProductMappingExtensions (Entity -> Contract)
    }

    public async Task<IEnumerable<ProductResponseContract>> GetAllProductsAsync()
    {
        var entities = await repository.GetAllProductsAsync();
        return entities.Select(p => p.AsContract());
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await repository.DeleteProductAsync(id);
    }

}

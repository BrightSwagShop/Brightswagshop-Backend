using System;
using FakeWebShop.Contracts;

namespace FakeWebShop.Domain.Services.Interfaces;

public interface IProductService
{
    
Task<ProductResponseContract?> GetProductAsync(Guid id);
Task<IEnumerable<ProductResponseContract>> GetAllProductsAsync();
Task DeleteProductAsync(Guid id);
}


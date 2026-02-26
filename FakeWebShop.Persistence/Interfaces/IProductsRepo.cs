using System;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Persistence.Interfaces;

public interface IProductsRepo
{


    Task<Product?> GetProductAsync(Guid id);
    Task<List<Product>> GetAllProductsAsync();
    Task DeleteProductAsync(Guid id);


}

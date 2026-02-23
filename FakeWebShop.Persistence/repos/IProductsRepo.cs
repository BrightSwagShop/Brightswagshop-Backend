using System;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Persistence.repos;

public interface IProductsRepo
{
    
    Task<Product> SaveProductAsync(Product product);
    Task<Product?> GetProductAsync(Guid id);
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid id);
    

}

using System;
using FakeWebShop.Persistence.Entities.Model;
using FakeWebShop.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FakeWebShop.Persistence.repos;

public class ProductsRepo(ProductDbContext dbContext) : IProductsRepo
{
    public async Task DeleteProductAsync(Guid id)
    {
        var existing = dbContext.Products.Find(id);
         if (existing is null)
         throw new Exception("Product not found");
          dbContext.Products.Remove(existing);
          await dbContext.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
         return await dbContext.Products.ToListAsync();
    }

    public async Task<Product?> GetProductAsync(Guid id)
    {
        return await dbContext.Products.FindAsync(id);
    }
 

     
}

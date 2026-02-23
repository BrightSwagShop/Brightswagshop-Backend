using System;
using FakeWebShop.Persistence.Entities.Model;
using Microsoft.EntityFrameworkCore;

namespace FakeWebShop.Persistence.repos;

public class ProductsRepo(ProductDbContext dbContext) : IProductsRepo
{
    public async Task DeleteProductAsync(Guid id)
    {
        var existing = dbContext.Categories.Find(id);
         if (existing is null)
         throw new Exception("Category not found");
          dbContext.Categories.Remove(existing);
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

    public async Task<Product> SaveProductAsync(Product product)
    {
         dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        var existing = dbContext.Products.Find(product.Id);
        if(existing is null)
        throw new Exception("product not found");
        existing.Naam = product.Naam;
        existing.Beschrijving = product.Beschrijving;
        existing.ImageUrl = product.ImageUrl;
        existing.CategoryId = product.CategoryId;
        await dbContext.SaveChangesAsync();
        return existing;
    }
}

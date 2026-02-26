using System;
using FakeWebShop.Persistence.Entities.Model;
using FakeWebShop.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace FakeWebShop.Persistence.repos;

public class CategoryRepo(ProductDbContext dbContext) : ICategoryRepo
{
    public async Task DeleteCategoryAsync(Guid id)
    {
        var existing = dbContext.Categories.Find(id);
        if (existing is null)
            throw new Exception("Category not found");
        dbContext.Categories.Remove(existing);
        await dbContext.SaveChangesAsync();

    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await dbContext.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(Guid id)
    {
        return await dbContext.Categories.FindAsync(id);
    }

    public async Task<Category> SaveCategoryAsync(Category category)
    {
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        var existing = dbContext.Categories.Find(category.Id);
        if (existing is null)
            throw new Exception("Category not found");
        existing.Naam = category.Naam;
        existing.Producten = category.Producten;
        await dbContext.SaveChangesAsync();
        return existing;

    }

    public async Task<List<Product>> GetAllProductsFromCategoryAsync(Guid categoryId)
    {
        return await dbContext.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

}

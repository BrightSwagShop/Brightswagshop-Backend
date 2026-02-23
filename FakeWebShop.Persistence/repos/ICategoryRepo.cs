using System;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Persistence.repos;

public interface ICategoryRepo
{
    Task<Category> SaveCategoryAsync(Category category);
    Task<Category?> GetCategoryAsync(Guid id);
   Task<List<Category>> GetAllCategoriesAsync();
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid id);
    Task<List<Product>> GetAllProductsFromCategoryAsync(Guid categoryId);

}

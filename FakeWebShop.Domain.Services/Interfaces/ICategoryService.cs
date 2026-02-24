using System;
using FakeWebShop.Contracts;

namespace FakeWebShop.Domain.Services.Interfaces;

public interface ICategoryService
{
     Task<CategoryResponseContract> CreateCategoryAsync(CategoryRequestContract category);

    Task<CategoryResponseContract?> GetCategoryAsync(Guid id);

    Task<IEnumerable<CategoryResponseContract>> GetAllCategoriesAsync();

    Task<CategoryResponseContract?> UpdateCategoryAsync(CategoryRequestContract category, Guid id);

    Task DeleteCategoryAsync(Guid id);

    Task<List<ProductResponseContract>> GetAllProductsFromCategoryAsync(Guid categoryId);

}

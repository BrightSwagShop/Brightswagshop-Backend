using System;
using FakeWebShop.Contracts;
using FakeWebShop.Domain.Services.Interfaces;
using FakeWebShop.Domain.Services.Mapping;
using FakeWebShop.Persistence.repos;

namespace FakeWebShop.Domain.Services.services;

public class CategoryService(ICategoryRepo repository) : ICategoryService
{
        public async Task<CategoryResponseContract> CreateCategoryAsync(CategoryRequestContract category)
    {
        var model = category.AsModel();
        var entity = model.AsEntity();

        var createdEntity = await repository.SaveCategoryAsync(entity);

        return createdEntity.AsModel().AsContract();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        await repository.DeleteCategoryAsync(id);
    }

    public async Task<IEnumerable<CategoryResponseContract>> GetAllCategoriesAsync()
    {
        var entities = await repository.GetAllCategoriesAsync();
        return entities.Select(c => c.AsModel().AsContract());
    }

    public async Task<CategoryResponseContract?> GetCategoryAsync(Guid id)
    {
        var entity = await repository.GetCategoryAsync(id);

        if (entity == null)
            return null;

        return entity.AsModel().AsContract();
    }

    public async Task<CategoryResponseContract?> UpdateCategoryAsync(CategoryRequestContract category, Guid id)
    {
        var model = category.AsModel();
        model.Id = id;

        try
        {
            var updatedEntity = await repository.UpdateCategoryAsync(model.AsEntity());
            return updatedEntity.AsModel().AsContract();
        }
        catch (Exception) // ideal: custom EntityNotFoundException
        {
            return null;
        }
    }

    public async Task<List<ProductResponseContract>> GetAllProductsFromCategoryAsync(Guid categoryId)
    {
        var products = await repository.GetAllProductsFromCategoryAsync(categoryId);

        // Je hebt nog Product mappings nodig (of per subtype). Dit is placeholder:
        return products.Select(p => p.AsContract()).ToList();
    }




}

using System;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Persistence.repos;

public interface IProductenRepo
{
     Task<T> SaveAsync<T>(T entity) where T : class;

    Task<T?> GetAsync<T>(Guid id) where T : class;

    Task<List<T>> GetAllAsync<T>() where T : class;

    Task<T> UpdateAsync<T>(Guid id, T entity) where T : class;

    Task DeleteAsync<T>(Guid id) where T : class;




}

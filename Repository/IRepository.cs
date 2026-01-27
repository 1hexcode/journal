using Journal.Models;

namespace Journal.Repository;

internal interface IRepository<TSource> where TSource : IModel, new()
{
    // Get all records
    Task<List<TSource>> GetAllAsync();

    // Get a single record by ID
    Task<TSource> GetAsync(Guid id);

    // Add or Update a record
    Task<int> SaveAsync(TSource entity);

    // Remove a record
    Task<int> DeleteAsync(TSource entity);
}
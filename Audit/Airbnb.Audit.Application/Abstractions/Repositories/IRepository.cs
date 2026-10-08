namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IRepository<TEntity>
{
    Task<TEntity> GetByIdAsync(string id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(string id, TEntity entity);
}
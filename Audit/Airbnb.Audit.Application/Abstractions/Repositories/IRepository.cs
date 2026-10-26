using Airbnb.Shared.HTTP;

namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IRepository<TEntity>
{
    Task<Result<TEntity>> GetByIdAsync(string id);
    
    Task CreateAsync(TEntity entity);
    
    Task UpdateAsync(string id, TEntity entity);

    Task DeleteAsync(string id);
}
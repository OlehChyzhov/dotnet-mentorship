using Airbnb.Contracts;

namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IRepository<TEntity>
{
    Task<Result<TEntity>> GetByIdAsync(string id);
    
    Task<Result<IEnumerable<TEntity>>> GetAllAsync();
    
    Task CreateAsync(TEntity entity);
    
    Task UpdateAsync(string id, TEntity entity);
}
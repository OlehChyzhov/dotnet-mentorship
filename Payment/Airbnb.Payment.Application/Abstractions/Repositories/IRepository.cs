using Airbnb.Payment.Domain;

namespace Airbnb.Payment.Application.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    Task<TEntity> GetByIdAsync(Guid id);
    Task<TEntity> GetByIdTrackedAsync(Guid id);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
}

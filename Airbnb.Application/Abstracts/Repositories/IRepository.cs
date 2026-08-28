using System.Linq.Expressions;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IRepository<TEntity, TKey, TExternalKey> where TEntity : class, IEntity<TKey, TExternalKey>
{
    Task<TEntity> GetByIdAsync(TKey id);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
}
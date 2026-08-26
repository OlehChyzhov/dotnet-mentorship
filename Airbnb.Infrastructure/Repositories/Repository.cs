using System.Linq.Expressions;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    protected readonly DbSet<TEntity> _dbSet;
    public Repository(ApplicationDbContext context)
    {
        _dbSet = context.Set<TEntity>();
    }
    
    public async Task<TEntity> GetByIdAsync(TKey id)
    {
        return await _dbSet.AsNoTracking().FirstAsync(entity => entity.Id!.Equals(id));
    }

    public async Task CreateAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    protected async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
    }
}
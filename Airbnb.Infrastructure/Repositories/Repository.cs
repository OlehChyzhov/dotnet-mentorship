using System.Linq.Expressions;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly DbSet<T> _dbSet;
    public Repository(ApplicationDbContext context)
    {
        _dbSet = context.Set<T>();
    }
    
    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbSet.AsNoTracking().FirstAsync(entity => entity.Id == id);
    }

    public async Task CreateAsync(T entity)
    {
        Guid entityGuid = Guid.NewGuid();
        if (entity.Id == Guid.Empty)
        {
            entity.Id = entityGuid;
        }
        
        await _dbSet.AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
    }
}
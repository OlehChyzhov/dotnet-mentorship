using System.Data.Common;
using System.Linq.Expressions;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Airbnb.Infrastructure.Database.Repositories;

public abstract class Repository<TEntity, TKey, TExternalKey> : IRepository<TEntity, TKey, TExternalKey>
    where TEntity : class, IEntity<TKey, TExternalKey>
{
    private readonly ApplicationDbContext _context;
    
    protected Repository(ApplicationDbContext context)
    {
        _context = context;
        DbSet = context.Set<TEntity>();
    }
    
    protected DbSet<TEntity> DbSet { get; }
    protected DbConnection Connection => _context.Database.GetDbConnection();
    protected DbTransaction? Transaction => _context.Database.CurrentTransaction?.GetDbTransaction();

    public async Task<TEntity> GetByIdAsync(TKey id)
    {
        return await DbSet.AsNoTracking().FirstAsync(entity => entity.Id!.Equals(id));
    }

    public async Task<TEntity> GetByExternalIdAsync(TExternalKey externalId)
    {
        return await DbSet.AsNoTracking().FirstAsync(entity => entity.ExternalId!.Equals(externalId));
    }

    public async Task CreateAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
    }

    public Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    protected async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.Where(predicate).AsNoTracking().ToListAsync();
    }
}
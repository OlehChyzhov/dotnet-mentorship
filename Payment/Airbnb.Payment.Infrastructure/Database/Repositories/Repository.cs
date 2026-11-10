using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Payment.Infrastructure.Database.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected Repository(ApplicationDbContext context)
    {
        DbSet = context.Set<TEntity>();
    }

    protected DbSet<TEntity> DbSet { get; }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        return await DbSet.AsNoTracking().FirstAsync(entity => entity.Id == id);
    }

    public async Task<TEntity> GetByIdTrackedAsync(Guid id)
    {
        return await DbSet.FirstAsync(entity => entity.Id == id);
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

    public Task DeleteAsync(Guid id)
    {
        var entityToDelete = DbSet.First(entity => entity.Id == id);
        DbSet.Remove(entityToDelete);
        return Task.CompletedTask;
    }
}

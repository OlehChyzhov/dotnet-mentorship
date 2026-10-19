using System.Linq.Expressions;
using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Contracts;
using Airbnb.Contracts.HTTP;
using Airbnb.Contracts.HTTP.Paging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public abstract class MongoRepository<T> : IRepository<T>
{
    protected MongoRepository(MongoDbContext context)
    {
        Collection = context.GetCollection<T>();
    }
    
    protected IMongoCollection<T> Collection { get; }
    
    public async Task<Result<T>> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await Collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var filter =  Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await Collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await Collection.DeleteOneAsync(filter);
    }
    
    protected async Task<PagedResult<T>> GetPagedAsync(
        FilterDefinition<T> filter, 
        SortDefinition<T> sort, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var totalCount = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        
        var audits = await Collection
            .Find(filter)
            .Sort(sort)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<T>(audits, (int)totalCount, pageNumber, pageSize);
    }
}
using Airbnb.Audit.Application.Abstractions.Repositories;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class MongoRepository<T> : IRepository<T>
{
    private readonly FilterDefinitionBuilder<T> _filterBuilder;
    private readonly IMongoCollection<T> _collection;
    
    protected MongoRepository(IMongoDatabase database, FilterDefinitionBuilder<T> filterBuilder)
    {
        _collection = database.GetCollection<T>(name: nameof(T));
        _filterBuilder = filterBuilder;
    }
    
    public async Task<T> GetByIdAsync(string id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var filter =  _filterBuilder.Eq("_id", id);
        await _collection.ReplaceOneAsync(filter, entity);
    }
}
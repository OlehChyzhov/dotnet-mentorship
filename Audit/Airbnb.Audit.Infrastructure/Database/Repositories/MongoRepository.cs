using Airbnb.Audit.Application.Abstractions.Repositories;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public abstract class MongoRepository<T> : IRepository<T>
{
    private readonly FilterDefinitionBuilder<T> _filterBuilder;
    private readonly IMongoCollection<T> _collection;
    
    protected MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(name: collectionName);
        _filterBuilder = Builders<T>.Filter;
    }
    
    public async Task<T> GetByIdAsync(string id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var audits = await _collection.FindAsync(_ => true);
        return audits.ToList();
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
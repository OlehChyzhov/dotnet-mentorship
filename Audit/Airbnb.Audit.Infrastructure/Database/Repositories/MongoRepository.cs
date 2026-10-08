using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public abstract class MongoRepository<T> : IRepository<T>
{
    protected readonly FilterDefinitionBuilder<T> _filterBuilder;
    protected readonly IMongoCollection<T> _collection;
    
    protected MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(name: collectionName);
        _filterBuilder = Builders<T>.Filter;
    }
    
    public async Task<Result<T>> GetByIdAsync(string id)
    {
        var filter = _filterBuilder.Eq("_id", ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Result<IEnumerable<T>>> GetAllAsync()
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
        var filter =  _filterBuilder.Eq("_id", ObjectId.Parse(id));
        await _collection.ReplaceOneAsync(filter, entity);
    }
}
using System.Reflection;
using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Attributes;
using Airbnb.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public abstract class MongoRepository<T> : IRepository<T>
{
    protected MongoRepository(IMongoDatabase database)
    {
        Collection = database.GetCollection<T>(GetCollectionName());
    }
    
    protected IMongoCollection<T> Collection { get; }
    
    public async Task<Result<T>> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Result<IEnumerable<T>>> GetAllAsync()
    {
        var audits = await Collection.FindAsync(_ => true);
        return audits.ToList();
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

    private string GetCollectionName()
    {
        var collectionName = typeof(T).GetCustomAttribute<BsonCollection>()?.CollectionName;
        if (string.IsNullOrEmpty(collectionName))
        {
            throw new InvalidOperationException($"{typeof(T).Name} does not have [BsonCollection] attribute");
        }
        
        return collectionName;
    }
}
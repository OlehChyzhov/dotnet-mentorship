using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Contracts;
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
}
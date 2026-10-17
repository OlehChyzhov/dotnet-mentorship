using System.Reflection;
using Airbnb.Audit.Domain.Attributes;
using Airbnb.Audit.Domain.Models;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database;

public class MongoDbContext : IDisposable
{
    private readonly IMongoDatabase _db;
    
    public MongoDbContext(IMongoDatabase db)
    {
        _db = db;
    }

    public IMongoCollection<UserAuditEntity> UserAudits => GetCollection<UserAuditEntity>();

    public IMongoCollection<T> GetCollection<T>()
    {
        return _db.GetCollection<T>(GetCollectionName<T>());
    }
    
    private string GetCollectionName<T>()
    {
        var collectionName = typeof(T).GetCustomAttribute<BsonCollection>()?.CollectionName;
        if (string.IsNullOrEmpty(collectionName))
        {
            throw new InvalidOperationException($"Type {typeof(T).Name} does not have a BsonCollection attribute)");
        }
        
        return collectionName;
    }

    public void Dispose() => _db.Client.Dispose();
}
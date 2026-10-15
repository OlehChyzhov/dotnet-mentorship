using System.Reflection;
using Airbnb.Audit.Domain.Attributes;
using Airbnb.Audit.Domain.Models;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Configurations;

public sealed class MongoDbIndexesConfiguration : IHostedService
{
    private readonly IMongoDatabase _database;
    
    public MongoDbIndexesConfiguration(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<UserAuditChangeEntity>(GetCollectionName());
        var keys = Builders<UserAuditChangeEntity>.IndexKeys;

        await collection.Indexes.CreateManyAsync([
            new CreateIndexModel<UserAuditChangeEntity>(keys.Ascending(x => x.UserId).Descending(x => x.CreatedAt), new CreateIndexOptions { Name = "ix_userId_createdAt"})
        ], cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    private string GetCollectionName()
    {
        var collectionName = typeof(UserAuditChangeEntity).GetCustomAttribute<BsonCollection>()?.CollectionName;
        if (string.IsNullOrEmpty(collectionName))
        {
            throw new InvalidOperationException($"{nameof(UserAuditChangeEntity)} does not have [BsonCollection] attribute");
        }
        
        return collectionName;
    }
}
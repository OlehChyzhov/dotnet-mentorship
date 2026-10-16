using Airbnb.Audit.Domain.Models;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.DbConfigs;

public sealed class MongoIndexConfig(MongoDbContext dbContext) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var keys = Builders<UserAuditChangeEntity>.IndexKeys;
        
        // User Audit Collection Indexes
        await dbContext.UserAudits.Indexes.CreateManyAsync([
            
            // Search by date
            new CreateIndexModel<UserAuditChangeEntity>(
                keys.Ascending(x => x.CreatedAt),
                new CreateIndexOptions { Name = "ix_createdAt" }),
            
            // Search by user id for the specified date
            new CreateIndexModel<UserAuditChangeEntity>(
                keys.Ascending(x => x.UserId).Ascending(x => x.CreatedAt), 
                new CreateIndexOptions { Name = "ix_userId_createdAt"}),
            
        ], cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
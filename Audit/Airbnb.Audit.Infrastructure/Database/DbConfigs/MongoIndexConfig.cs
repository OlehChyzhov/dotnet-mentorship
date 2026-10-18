using Airbnb.Audit.Domain.Models;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.DbConfigs;

public sealed class MongoIndexConfig(MongoDbContext dbContext) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // User Audit Collection Indexes
        var userIndexBuilder = Builders<UserAuditEntity>.IndexKeys;
        await dbContext.UserAudits.Indexes.CreateManyAsync([
            new CreateIndexModel<UserAuditEntity>(
                userIndexBuilder.Ascending(x => x.EventOccuredAt),
                new CreateIndexOptions { Name = "ix_eventOccuredAt" }),
            
            new CreateIndexModel<UserAuditEntity>(
                userIndexBuilder.Ascending(x => x.UserId).Ascending(x => x.EventOccuredAt), 
                new CreateIndexOptions { Name = "ix_userId_eventOccuredAt"}),
            
        ], cancellationToken);

        // Apartment Audit Collection Indexes
        var apartmentIndexBuilder = Builders<ApartmentAuditEntity>.IndexKeys;
        await dbContext.ApartmentAudits.Indexes.CreateManyAsync([
            new CreateIndexModel<ApartmentAuditEntity>(
                apartmentIndexBuilder.Ascending(x => x.EventOccuredAt), 
                new CreateIndexOptions() { Name = "ix_eventOccuredAt" }),
            
            new CreateIndexModel<ApartmentAuditEntity>(
                apartmentIndexBuilder.Ascending(x => x.ApartmentId).Ascending(x => x.EventOccuredAt), 
                new CreateIndexOptions() { Name = "ix_apartmentId_eventOccuredAt"})
        ], cancellationToken);
        
        // Booking Audit Collection Indexes
        var bookingIndexBuilder = Builders<BookingAuditEntity>.IndexKeys;
        await dbContext.BookingAudits.Indexes.CreateManyAsync([
            new CreateIndexModel<BookingAuditEntity>(
                bookingIndexBuilder.Ascending(x => x.EventOccuredAt), 
                new CreateIndexOptions() { Name = "ix_eventOccuredAt" }),
            
            new CreateIndexModel<BookingAuditEntity>(
                bookingIndexBuilder.Ascending(x => x.BookingId).Ascending(x => x.EventOccuredAt), 
                new CreateIndexOptions() { Name = "ix_bookingId_eventOccuredAt"})
        ], cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
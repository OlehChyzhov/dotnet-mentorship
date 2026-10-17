using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts;
using Airbnb.Contracts.HTTP.Paging;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class UserAuditRepository : MongoRepository<UserAuditEntity>, IUserAuditRepository
{
    public UserAuditRepository(MongoDbContext db): base(db) { }

    public async Task<PagedResult<UserAuditEntity>> GetAllAuditsPaged(UserAuditFilter parameters)
    {
        var filterBuilder = Builders<UserAuditEntity>.Filter;
        var filter = filterBuilder.Empty;
        
        if (parameters.UserId != null)
        {
            filter &= filterBuilder.Eq(audit => audit.UserId, parameters.UserId);
        }

        if (parameters.Date != null)
        {
            var dayStart = DateTime.SpecifyKind(parameters.Date.Value.Date, DateTimeKind.Utc);
            var dayEnd = dayStart.AddDays(1);

            filter &= filterBuilder.Gte(audit => audit.CreatedAt, dayStart)
                      & filterBuilder.Lt(audit => audit.CreatedAt, dayEnd);
        }

        var totalCount = await Collection.CountDocumentsAsync(filter);

        var audits = await Collection
            .Find(filter)
            .SortByDescending(audit => audit.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Limit(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<UserAuditEntity>(
            items: audits, 
            totalCount: (int)totalCount, 
            pageNumber: parameters.PageNumber, 
            pageSize: parameters.PageSize
        );
    }
}
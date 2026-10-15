using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts;
using Airbnb.Contracts.Paging;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class UserAuditRepository : MongoRepository<UserAuditChangeEntity>, IUserAuditRepository
{
    public UserAuditRepository(IMongoDatabase db): base(database: db, collectionName: "user_audits") { }

    public async Task<PagedResult<UserAuditChangeEntity>> GetAllAuditsPaged(UserAuditFilter parameters)
    {
        var filterBuilder = Builders<UserAuditChangeEntity>.Filter;
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

        return new PagedResult<UserAuditChangeEntity>(
            items: audits, 
            totalCount: (int)totalCount, 
            pageNumber: parameters.PageNumber, 
            pageSize: parameters.PageSize
        );
    }
}
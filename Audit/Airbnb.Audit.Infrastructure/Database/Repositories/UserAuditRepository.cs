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

    public async Task<PagedList<UserAuditChangeEntity>> GetAllAuditsPaged(UserAuditPagingParameters parameters)
    {
        var filter = _filterBuilder.Empty;

        if (parameters.UserId != null)
        {
            filter &= _filterBuilder.Eq(audit => audit.UserId, parameters.UserId);
        }

        if (parameters.Date != null)
        {
            var dayStart = DateTime.SpecifyKind(parameters.Date.Value.Date, DateTimeKind.Utc);
            var dayEnd = dayStart.AddDays(1);

            filter &= _filterBuilder.Gte(audit => audit.CreatedAt, dayStart)
                      & _filterBuilder.Lt(audit => audit.CreatedAt, dayEnd);
        }

        var totalCount = await _collection.CountDocumentsAsync(filter);

        var audits = await _collection
            .Find(filter)
            .SortByDescending(audit => audit.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Limit(parameters.PageSize)
            .ToListAsync();

        return PagedList<UserAuditChangeEntity>.ToPagedList(
            audits, (int)totalCount, parameters.PageNumber, parameters.PageSize);
    }
}
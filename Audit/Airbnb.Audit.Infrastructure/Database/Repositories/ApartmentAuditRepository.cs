using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.HTTP;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class ApartmentAuditRepository : MongoRepository<ApartmentAuditEntity>, IApartmentAuditRepository
{
    public ApartmentAuditRepository(MongoDbContext db) : base(db) { }
    
    public async Task<PagedResult<ApartmentAuditEntity>> GetApartmentsPagedAsync(ApartmentAuditFilter props)
    {
        var filterBuilder = Builders<ApartmentAuditEntity>.Filter;
        var filter = filterBuilder.Empty;
        
        if (props.ApartmentId != null)
        {
            filter &= filterBuilder.Eq(audit => audit.ApartmentId, props.ApartmentId);
        }

        if (props.Date != null)
        {
            var startDate = DateTime.SpecifyKind(props.Date.Value.Date, DateTimeKind.Utc);
            var endDate = startDate.AddDays(1);

            filter &= filterBuilder.Gte(audit => audit.EventOccuredAt, startDate) &
                      filterBuilder.Lt(audit => audit.EventOccuredAt, endDate);
        }

        var sort = Builders<ApartmentAuditEntity>.Sort.Descending(audit => audit.EventOccuredAt);
        return await GetPagedAsync(filter, sort, props.PageNumber, props.PageSize);
    }
}
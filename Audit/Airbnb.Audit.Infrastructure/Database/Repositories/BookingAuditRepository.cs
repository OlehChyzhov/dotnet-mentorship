using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.HTTP;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class BookingAuditRepository : MongoRepository<BookingAuditEntity>, IBookingAuditRepository
{
    public BookingAuditRepository(MongoDbContext db) : base(db) { }
    
    public async Task<PagedResult<BookingAuditEntity>> GetBookingsPagedAsync(BookingAuditFilter props)
    {
        var filterBuilder = Builders<BookingAuditEntity>.Filter;
        var filter = filterBuilder.Empty;
        
        if (props.BookingId != null)
        {
            filter &= filterBuilder.Eq(audit => audit.BookingId, props.BookingId);
        }

        if (props.Date != null)
        {
            var startDate = DateTime.SpecifyKind(props.Date.Value.Date, DateTimeKind.Utc);
            var endDate = startDate.AddDays(1);

            filter &= filterBuilder.Gte(audit => audit.EventOccuredAt, startDate) &
                      filterBuilder.Lt(audit => audit.EventOccuredAt, endDate);
        }

        var sort = Builders<BookingAuditEntity>.Sort.Descending(audit => audit.EventOccuredAt);
        return await GetPagedAsync(filter, sort, props.PageNumber, props.PageSize);
    }
}
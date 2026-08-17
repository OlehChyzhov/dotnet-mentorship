using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IBookingRepository : IRepository<Domain.Models.Booking>
{
    Task<PagedList<Domain.Models.Booking>> GetBookingsPagedAsync(BookingPagingParameters query, string userId);
    
    Task<List<Domain.Models.Booking>> GetConfirmedOrPendingBookingsInTimeRangeAsync(Guid apartmentId, DateTime from, DateTime to);
}
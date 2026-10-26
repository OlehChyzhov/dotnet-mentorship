using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Domain.Models;
using Airbnb.Shared.HTTP;

namespace Airbnb.Application.Abstractions.Repositories;

public interface IBookingRepository : IRepository<Booking, Guid, Guid?>
{
    Task<PagedResult<Booking>> GetBookingsPagedAsync(BookingPagingPrams query, string userId);
    
    Task<List<Booking>> GetConfirmedOrPendingBookingsInTimeRangeAsync(Guid apartmentId, DateTime from, DateTime to);
}
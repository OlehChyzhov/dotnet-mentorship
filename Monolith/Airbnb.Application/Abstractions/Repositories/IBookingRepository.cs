using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Contracts.Paging;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstractions.Repositories;

public interface IBookingRepository : IRepository<Booking, Guid, Guid?>
{
    Task<PagedResult<Booking>> GetBookingsPagedAsync(BookingPagingPrams query, string userId);
    
    Task<List<Booking>> GetConfirmedOrPendingBookingsInTimeRangeAsync(Guid apartmentId, DateTime from, DateTime to);
}
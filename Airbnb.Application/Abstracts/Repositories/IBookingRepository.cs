using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IBookingRepository : IRepository<Booking, Guid, Guid>
{
    Task<PagedList<Booking>> GetBookingsPagedAsync(BookingPagingParameters query, string userId);
    
    Task<List<Booking>> GetConfirmedOrPendingBookingsInTimeRangeAsync(Guid apartmentId, DateTime from, DateTime to);
}
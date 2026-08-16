using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<PagedList<Booking>> GetBookingsPagedAsync(BookingParameters parameters);
}
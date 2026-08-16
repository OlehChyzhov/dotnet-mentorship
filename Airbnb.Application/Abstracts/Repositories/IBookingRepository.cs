using Airbnb.Domain.Models;
using Airbnb.Domain.Requests.Paging;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<PagedList<Booking>> GetBookingsPagedAsync(BookingParameters parameters);
}
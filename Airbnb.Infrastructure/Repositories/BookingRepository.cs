using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class BookingRepository : Repository<Domain.Models.Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) {}

    public async Task<PagedList<Domain.Models.Booking>> GetBookingsPagedAsync(BookingPagingParameters query, string userId)
    {
        var userBookings = _dbSet
            .Where(booking => booking.ClientId == userId)
            .OrderBy(booking => booking.CheckIn);

        var totalCount = await userBookings.CountAsync();

        var bookings = await userBookings
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return PagedList<Domain.Models.Booking>.ToPagedList(bookings, totalCount, query.PageNumber, query.PageSize);
    }

    public async Task<List<Domain.Models.Booking>> GetConfirmedOrPendingBookingsInTimeRangeAsync(Guid apartmentId, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(booking => booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Pending)
            .Where(booking => booking.CheckIn < to && booking.CheckOut > from)
            .Where(booking => booking.ApartmentId == apartmentId)
            .OrderBy(booking => booking.CheckIn)
            .ToListAsync();
    }
}
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) {}

    public async Task<PagedList<Booking>> GetBookingsPagedAsync(BookingParameters parameters, string userId)
    {
        var totalCount = await _dbSet.CountAsync();

        var bookings = await _dbSet
            .Where(booking => booking.ClientId == userId)
            .OrderBy(booking => booking.CheckIn)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return PagedList<Booking>.ToPagedList(bookings, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<List<Booking>> GetConfirmedOrPendingBookingsInTimeRangeAsync(Guid apartmentId, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(booking => booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Pending)
            .Where(booking => booking.CheckIn >= from && booking.CheckIn <= to)
            .Where(booking => booking.ApartmentId == apartmentId)
            .OrderBy(booking => booking.CheckIn)
            .ToListAsync();
    }
}
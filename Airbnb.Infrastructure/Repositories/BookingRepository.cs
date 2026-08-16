using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) {}

    public async Task<PagedList<Booking>> GetBookingsPagedAsync(BookingParameters parameters)
    {
        var totalCount = await _dbSet.CountAsync();

        var bookings = await _dbSet
            .OrderBy(booking => booking.CheckIn)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return PagedList<Booking>.ToPagedList(bookings, totalCount, parameters.PageNumber, parameters.PageSize);
    }
}
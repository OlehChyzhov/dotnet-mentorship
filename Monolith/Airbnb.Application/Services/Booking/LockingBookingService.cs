using Airbnb.Application.Abstractions.Services;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Shared.Common;

namespace Airbnb.Application.Services.Booking;

public class LockingBookingService : IBookingService
{
    private readonly IBookingService _defaultService;
    private readonly ICacheBasedOperationLocker _locker;
    
    public LockingBookingService(
        IBookingService defaultService,
        ICacheBasedOperationLocker locker)
    {
        _defaultService = defaultService;
        _locker = locker;
    }
    
    public Task<Result<BookingDto>> GetBookingByIdAsync(Guid bookingId)
    {
        return _defaultService.GetBookingByIdAsync(bookingId);
    }

    public Task<Result<PagedResult<BookingDto>>> GetBookingsAsync(BookingPagingPrams query, string userId)
    {
        return _defaultService.GetBookingsAsync(query, userId);
    }

    public async Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto dto, string userId)
    {
        var key = $"lock:booking:apartment:{dto.ApartmentId}";
        var lockingToken = Guid.NewGuid().ToString();

        if (!await _locker.LockAsync(key, lockingToken, TimeSpan.FromSeconds(30)))
        {
            return "The apartment is being booked by someone else right now, try again";
        }

        try
        {
            return await _defaultService.CreateBookingAsync(dto, userId);
        }
        finally
        {
            await  _locker.ReleaseAsync(key, lockingToken);
        }
    }
}
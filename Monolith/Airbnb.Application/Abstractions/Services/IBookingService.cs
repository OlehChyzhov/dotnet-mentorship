using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Shared.HTTP;

namespace Airbnb.Application.Abstractions.Services;

public interface IBookingService
{
    Task<Result<BookingDto>> GetBookingByIdAsync(Guid bookingId);
    
    Task<Result<PagedResult<BookingDto>>> GetBookingsAsync(BookingPagingPrams query, string userId);

    Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto dto, string userId);
}
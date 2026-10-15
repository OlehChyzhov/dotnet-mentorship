using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Contracts;
using Airbnb.Contracts.Paging;

namespace Airbnb.Application.Abstractions.Services;

public interface IBookingService
{
    Task<Result<BookingDto>> GetBookingByIdAsync(Guid bookingId);
    
    Task<Result<PagedResult<BookingDto>>> GetBookingsAsync(BookingPagingPrams query, string userId);

    Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto dto, string userId);
}
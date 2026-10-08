using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Contracts;
using Airbnb.Contracts.Paging;
using Airbnb.Domain;

namespace Airbnb.Application.Abstracts.Services;

public interface IBookingService
{
    Task<Result<BookingDto>> GetBookingByIdAsync(Guid bookingId);
    
    Task<Result<PagedList<BookingDto>>> GetBookingsAsync(BookingPagingParameters query, string userId);

    Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto dto, string userId);
}
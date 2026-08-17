using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;

namespace Airbnb.Application.Abstracts.Services;

public interface IBookingService
{
    Task<BookingDto> GetBookingByIdAsync(Guid bookingId);
    
    Task<(List<BookingDto> bookings, PagingMetaData metadata)> GetBookingsAsync(BookingPagingParameters query, string userId);

    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, string userId);
}
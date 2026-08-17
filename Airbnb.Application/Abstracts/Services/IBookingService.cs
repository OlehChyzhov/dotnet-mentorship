using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;

namespace Airbnb.Application.Abstracts.Services;

public interface IBookingService
{
    Task<(List<BookingDto> bookings, PagingMetaData metadata)> GetBookingsAsync(BookingParameters parameters, string userId);

    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, string userId);
}
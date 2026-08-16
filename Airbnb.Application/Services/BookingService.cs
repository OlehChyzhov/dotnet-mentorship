using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Exceptions;
using Airbnb.Domain.Models;
using MapsterMapper;

namespace Airbnb.Application.Services;

public class BookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<(List<BookingDto> bookings, PagingMetaData metadata)> GetBookingsAsync(BookingParameters parameters, string userId)
    {
        var bookingsWithMetaData = await _unitOfWork.Bookings.GetBookingsPagedAsync(parameters, userId);
        var bookingsDto = _mapper.Map<List<BookingDto>>(bookingsWithMetaData);

        return (bookingsDto, bookingsWithMetaData.MetaData);
    }

    public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, string userId)
    {
        var bookingsInTimeRange = await _unitOfWork.Bookings
            .GetConfirmedOrPendingBookingsInTimeRangeAsync(dto.ApartmentId, dto.CheckIn, dto.CheckOut);

        if (bookingsInTimeRange.Any())
        {
            throw new BookingConflictException("The apartment is already booked for the selected time range");
        }
        
        Guid bookingId = Guid.NewGuid();
        var booking = _mapper.Map<Booking>(dto);

        booking.Id = bookingId;
        booking.ClientId = userId;
        
        await _unitOfWork.Bookings.CreateAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        
        var createdBooking = _unitOfWork.Bookings.GetByIdAsync(bookingId);
        var createdBookingDto = _mapper.Map<BookingDto>(createdBooking);
        
        return createdBookingDto;
    }
}
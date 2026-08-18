using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Exceptions;
using Airbnb.Domain.Models;
using MapsterMapper;

namespace Airbnb.Application.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<BookingDto>> GetBookingByIdAsync(Guid bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        var bookingDto = _mapper.Map<BookingDto>(booking);
        return bookingDto;
    }
    
    public async Task<Result<PagedList<BookingDto>>> GetBookingsAsync(BookingPagingParameters query, string userId)
    {
        var bookingsWithMetaData = await _unitOfWork.Bookings.GetBookingsPagedAsync(query, userId);

        var pagingMetaData = bookingsWithMetaData.MetaData;
        var bookingsDto = _mapper.Map<List<BookingDto>>(bookingsWithMetaData);
        
        var bookingsResult = PagedList<BookingDto>.ToPagedList(
            source: bookingsDto,
            totalCount: pagingMetaData.TotalCount,
            pageNumber: pagingMetaData.CurrentPage,
            pageSize: pagingMetaData.PageSize);

        return bookingsResult;
    }

    public async Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto dto, string userId)
    {
        var apartment = await _unitOfWork.Apartments.GetByIdAsync(dto.ApartmentId);
        var bookingsInTimeRange = await _unitOfWork.Bookings
            .GetConfirmedOrPendingBookingsInTimeRangeAsync(dto.ApartmentId, dto.CheckIn, dto.CheckOut);

        if (bookingsInTimeRange.Any())
        {
            return "The apartment is already booked for the selected time range";
        }
        else if (!apartment.IsListed)
        {
            return "Can't create booking for an unlisted apartment";
        }
        

        Guid bookingId = Guid.NewGuid();
        var booking = _mapper.Map<Domain.Models.Booking>(dto);

        booking.Id = bookingId;
        booking.ClientId = userId;
        booking.Status = BookingStatus.Pending;
        booking.CreatedAt = DateTime.UtcNow;
        booking.BookedPricePerNight = apartment.PricePerNight;
        booking.BookedTotalPrice =  apartment.PricePerNight * ((dto.CheckOut - dto.CheckIn).TotalDays);
        
        await _unitOfWork.Bookings.CreateAsync(booking);
        await _unitOfWork.SaveChangesAsync();
        
        var createdBooking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        var createdBookingDto = _mapper.Map<BookingDto>(createdBooking);
        
        return createdBookingDto;
    }
}
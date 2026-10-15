using Airbnb.Application.Abstractions.Services;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Contracts;
using Airbnb.Contracts.Paging;
using Airbnb.Domain;
using Airbnb.Domain.Enums;
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
    
    public async Task<Result<PagedResult<BookingDto>>> GetBookingsAsync(BookingPagingPrams query, string userId)
    {
        var bookingsWithMetaData = await _unitOfWork.Bookings.GetBookingsPagedAsync(query, userId);
        var bookingsDto = _mapper.Map<List<BookingDto>>(bookingsWithMetaData.Items);
        
        var bookingsResult = new PagedResult<BookingDto>(
            items: bookingsDto,
            totalCount: bookingsWithMetaData.Metadata.TotalCount,
            pageNumber: bookingsWithMetaData.Metadata.CurrentPage,
            pageSize: bookingsWithMetaData.Metadata.PageSize);

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
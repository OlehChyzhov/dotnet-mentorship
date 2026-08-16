using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs;
using Airbnb.Domain.Requests.Paging;
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

    public async Task<(List<BookingDto> bookings, PagingMetaData metadata)> GetBookingsAsync(BookingParameters parameters)
    {
        var bookingsWithMetaData = await _unitOfWork.Bookings.GetBookingsPagedAsync(parameters);
        var bookingsDto = _mapper.Map<List<BookingDto>>(bookingsWithMetaData);

        return (bookingsDto, bookingsWithMetaData.MetaData);
    }
}
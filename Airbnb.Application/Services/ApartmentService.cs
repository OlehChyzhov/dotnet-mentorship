using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;
using MapsterMapper;

namespace Airbnb.Application.Services;

public class ApartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public ApartmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<(List<ApartmentDto> apartments, PagingMetaData metadata)> GetApartmentsAsync(ApartmentParameters parameters)
    {
        var apartmentsWithMetaData = await _unitOfWork.Apartments.GetApartmentsPagedAsync(parameters);
        var apartmentsDto = _mapper.Map<List<ApartmentDto>>(apartmentsWithMetaData);
        
        return (apartmentsDto, apartmentsWithMetaData.MetaData);
    }

    public async Task CreateApartmentAsync(CreateApartmentDto dto)
    {
        var apartment = _mapper.Map<Apartment>(dto);
        await _unitOfWork.Apartments.CreateAsync(apartment);
        await _unitOfWork.SaveChangesAsync();
    }
}
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;
using MapsterMapper;

namespace Airbnb.Application.Services;

public class ApartmentService : IApartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApartmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApartmentDto> GetApartmentByIdAsync(Guid id)
    {
        var apartment = await _unitOfWork.Apartments.GetByIdAsync(id);
        var apartmentDto = _mapper.Map<ApartmentDto>(apartment);
        return apartmentDto;
    }

    public async Task<(List<ApartmentDto> apartments, PagingMetaData metadata)> GetApartmentsAsync(ApartmentPagingParamters query)
    {
        var apartmentsWithMetaData = await _unitOfWork.Apartments.GetApartmentsPagedAsync(query);
        var apartmentsDto = _mapper.Map<List<ApartmentDto>>(apartmentsWithMetaData);
        
        return (apartmentsDto, apartmentsWithMetaData.MetaData);
    }

    public async Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto, string userId)
    {
        var apartmentGuid = Guid.NewGuid();
        var apartment = _mapper.Map<Domain.Models.Apartment>(dto);
        
        apartment.Id = apartmentGuid;
        apartment.OwnerId = userId;
        apartment.CreatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Apartments.CreateAsync(apartment);
        await _unitOfWork.SaveChangesAsync();
        
        var createdApartment = await _unitOfWork.Apartments.GetByIdAsync(apartmentGuid);
        var createdApartmentDto = _mapper.Map<ApartmentDto>(createdApartment);

        return createdApartmentDto;
    }
}
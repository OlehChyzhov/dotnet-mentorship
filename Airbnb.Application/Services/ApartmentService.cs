using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain;
using Airbnb.Domain.Enums;
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

    public async Task<Result<ApartmentDto>> GetApartmentByIdAsync(Guid id)
    {
        var apartment = await _unitOfWork.Apartments.GetByIdAsync(id);
        var apartmentDto = _mapper.Map<ApartmentDto>(apartment);
        
        return apartmentDto;
    }

    public async Task<Result<PagedList<ApartmentDto>>> GetApartmentsAsync(ApartmentPagingParamters query)
    {
        var apartmentsWithMetaData = await _unitOfWork.Apartments.GetApartmentsPagedAsync(query);

        var pagingMetaData = apartmentsWithMetaData.MetaData;
        var apartmentsDto = _mapper.Map<List<ApartmentDto>>(apartmentsWithMetaData);
        
        var apartmentsResult = PagedList<ApartmentDto>.ToPagedList(
            source: apartmentsDto, 
            totalCount: pagingMetaData.TotalCount, 
            pageNumber: pagingMetaData.CurrentPage, 
            pageSize: pagingMetaData.PageSize);

        return apartmentsResult;
    }

    public async Task<Result<ApartmentDto>> GetApartmentByExternalIdAsync(Guid id)
    {
        var apartment = await _unitOfWork.Apartments.GetByExternalIdAsync(id);
        var apartmentDto = _mapper.Map<ApartmentDto>(apartment);
        
        return apartmentDto;
    }

    public async Task<Result<ApartmentDto>> CreateApartmentAsync(CreateApartmentDto dto, string userId)
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

    public async Task<Result<ApartmentDto>> UpsertApartmentAsync(UpsertApartmentDto dto, string userId)
    {
        var apartment = _mapper.Map<Apartment>(dto);
        apartment.OwnerId = userId;

        var upsertResult = await _unitOfWork.Apartments.UpsertApartmentAsync(apartment);
        if (!upsertResult.IsSuccessful)
        {
            return upsertResult.Message!;
        }

        return _mapper.Map<ApartmentDto>(upsertResult.Value!);
    }

    public async Task<Result<List<ApartmentByProfitDto>>> GetTopApartmentsByProfitAsync(int numOfApartments, string userId) =>
        await _unitOfWork.Apartments.GetTopApartmentsByProfitAsync(numOfApartments, userId);

    public async Task<Result<List<ApartmentCityAveragesDto>>> GetAverageApartmentCountAndPricePerCityAsync(string city) =>
        await _unitOfWork.Apartments.GetAverageApartmentCountAndPricePerCityAsync(city);

    public async Task<Result<List<ApartmentTypeAveragesDto>>> GetAverageApartmentPricePerTypeAsync(ApartmentType type) =>
        await _unitOfWork.Apartments.GetAverageApartmentPricePerTypeAsync(type);

    public async Task<Result<List<ApartmentTypeBookingStatsDto>>> GetApartmentBookingCountAndAverageStayByTypeAsync(ApartmentType type) =>
        await _unitOfWork.Apartments.GetApartmentBookingCountAndAverageStayByTypeAsync(type);

    public async Task<Result<List<ApartmentBedroomsAveragesDto>>> GetApartmentCountAndAveragePriceByBedroomsAsync(int numOfBedrooms) =>
        await _unitOfWork.Apartments.GetApartmentCountAndAveragePriceByBedroomsAsync(numOfBedrooms);
}
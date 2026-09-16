using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain;
using Airbnb.Domain.Enums;

namespace Airbnb.Application.Abstracts.Services;

public interface IApartmentService
{
    Task<Result<ApartmentDto>> GetApartmentByIdAsync(Guid id);

    Task<Result<ApartmentDto>> GetApartmentByExternalIdAsync(Guid id);

    Task<Result<PagedList<ApartmentDto>>> GetApartmentsAsync(ApartmentPagingParamters query);

    Task<Result<ApartmentDto>> CreateApartmentAsync(CreateApartmentDto dto, string userId);

    Task<Result<ApartmentDto>> UpsertApartmentAsync(UpsertApartmentDto dto, string userId);

    Task<Result<List<ApartmentByProfitDto>>> GetTopApartmentsByProfitAsync(int numOfApartments, string userId);

    Task<Result<List<ApartmentCityAveragesDto>>> GetAverageApartmentCountAndPricePerCityAsync(string city);

    Task<Result<List<ApartmentTypeAveragesDto>>> GetAverageApartmentPricePerTypeAsync(ApartmentType type);

    Task<Result<List<ApartmentTypeBookingStatsDto>>> GetApartmentBookingCountAndAverageStayByTypeAsync(ApartmentType type);

    Task<Result<List<ApartmentBedroomsAveragesDto>>> GetApartmentCountAndAveragePriceByBedroomsAsync(int numOfBedrooms);
}
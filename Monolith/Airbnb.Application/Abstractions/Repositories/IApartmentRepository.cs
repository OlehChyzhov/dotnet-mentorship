using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Contracts;
using Airbnb.Contracts.HTTP;
using Airbnb.Contracts.HTTP.Paging;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstractions.Repositories;

public interface IApartmentRepository : IRepository<Apartment, Guid, Guid?>
{
    Task<PagedResult<Apartment>> GetApartmentsPagedAsync(ApartmentPagingParamters query);

    Task<Result<Apartment>> UpsertApartmentAsync(Apartment apartmentToUpsert);

    Task<Result<List<ApartmentByProfitDto>>> GetTopApartmentsByProfitAsync(int numOfApartments, string userId);

    Task<Result<List<ApartmentCityAveragesDto>>> GetAverageApartmentCountAndPricePerCityAsync(string city);

    Task<Result<List<ApartmentTypeAveragesDto>>> GetAverageApartmentPricePerTypeAsync(ApartmentType type);

    Task<Result<List<ApartmentTypeBookingStatsDto>>> GetApartmentBookingCountAndAverageStayByTypeAsync(ApartmentType type);

    Task<Result<List<ApartmentBedroomsAveragesDto>>> GetApartmentCountAndAveragePriceByBedroomsAsync(int numOfBedrooms);
}
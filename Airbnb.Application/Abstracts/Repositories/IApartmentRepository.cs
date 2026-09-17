using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IApartmentRepository : IRepository<Apartment, Guid, Guid?>
{
    Task<PagedList<Apartment>> GetApartmentsPagedAsync(ApartmentPagingParamters query);

    Task<Result<Apartment>> UpsertApartmentAsync(Apartment apartmentToUpsert);

    Task<Result<List<ApartmentByProfitDto>>> GetTopApartmentsByProfitAsync(int numOfApartments, string userId);

    Task<Result<List<ApartmentCityAveragesDto>>> GetAverageApartmentCountAndPricePerCityAsync(string city);

    Task<Result<List<ApartmentTypeAveragesDto>>> GetAverageApartmentPricePerTypeAsync(ApartmentType type);

    Task<Result<List<ApartmentTypeBookingStatsDto>>> GetApartmentBookingCountAndAverageStayByTypeAsync(ApartmentType type);

    Task<Result<List<ApartmentBedroomsAveragesDto>>> GetApartmentCountAndAveragePriceByBedroomsAsync(int numOfBedrooms);
}
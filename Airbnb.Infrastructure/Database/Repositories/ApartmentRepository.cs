using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;
using Airbnb.Infrastructure.Database.Dapper;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Database.Repositories;

public class ApartmentRepository : Repository<Apartment, Guid, Guid?>, IApartmentRepository
{
    public ApartmentRepository(ApplicationDbContext context) : base(context) {}
    
    public async Task<PagedList<Apartment>> GetApartmentsPagedAsync(ApartmentPagingParamters query)
    {
        var apartmentsQuery = _dbSet.AsQueryable();

        // All apartments where there are no pending or confirmed bookings for a time range in parameters
        if (query.StartDate != null && query.EndDate != null)
        {
            apartmentsQuery = apartmentsQuery
                .Include(apartment => apartment.Bookings)
                .Where(apartment => apartment.Bookings
                    .Where(booking =>
                        booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Pending)
                    .All(booking => booking.CheckOut <= query.StartDate || booking.CheckIn >= query.EndDate)
                );

        }

        // Apartments that have specified IsListed value
        if (query.IsListed != null)
        {
            apartmentsQuery = apartmentsQuery
                .Where(apartment => apartment.IsListed == query.IsListed);
        }

        // Order apartments by Title
        apartmentsQuery = apartmentsQuery
            .OrderBy(apartment => apartment.Title);

        var totalCount = await apartmentsQuery.CountAsync();

        var apartments = await apartmentsQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return PagedList<Apartment>.ToPagedList(apartments, totalCount, query.PageNumber, query.PageSize);
    }

    public async Task<Result<Apartment>> UpsertApartmentAsync(Apartment apartmentToUpsert)
    {
        string? query =  QueryReader.GetQuery("UpsertApartment");
        if (query == null)
        {
            return"No query exists";
        }

        var apartment = await Connection.QueryAsync<Apartment>(query, apartmentToUpsert);
        if (!apartment.Any())
        {
            return "Could not upsert the apartment";
        }
        
        return apartment.First();
    }

    public async Task<Result<List<ApartmentByProfitDto>>> GetTopApartmentsByProfitAsync(int numOfApartments, string userId)
    {
        string? query = QueryReader.GetQuery("GetTopApartmentsByProfit");
        if (query == null)
        {
            return "No results found";
        }
        
        var apartments = await Connection.QueryAsync<ApartmentByProfitDto>(query, new { Count = numOfApartments, UserId = userId });
        return apartments.ToList();
    }

    public async Task<Result<List<ApartmentCityAveragesDto>>> GetAverageApartmentCountAndPricePerCityAsync(string city)
    {
        string? query = QueryReader.GetQuery("GetAverageApartmentCountAndPricePerCity");
        if (query == null)
        {
            return "No results found";
        }

        var statistics = await Connection.QueryAsync<ApartmentCityAveragesDto>(query, new { City = city });
        return statistics.ToList();
    }

    public async Task<Result<List<ApartmentTypeAveragesDto>>> GetAverageApartmentPricePerTypeAsync(ApartmentType type)
    {
        string? query = QueryReader.GetQuery("GetAverageApartmentPricePerType");
        if (query == null)
        {
            return "No results found";
        }

        var statistics = await Connection.QueryAsync<ApartmentTypeAveragesDto>(query, new { Type = type });
        return statistics.ToList();
    }

    public async Task<Result<List<ApartmentTypeBookingStatsDto>>> GetApartmentBookingCountAndAverageStayByTypeAsync(ApartmentType type)
    {
        string? query = QueryReader.GetQuery("GetApartmentBookingCountAndAverageStayByType");
        if (query == null)
        {
            return "No results found";
        }

        var statistics = await Connection.QueryAsync<ApartmentTypeBookingStatsDto>(query, new { Type = type });
        return statistics.ToList();
    }

    public async Task<Result<List<ApartmentBedroomsAveragesDto>>> GetApartmentCountAndAveragePriceByBedroomsAsync(int numOfBedrooms)
    {
        string? query = QueryReader.GetQuery("GetApartmentCountAndAveragePriceByBedrooms");
        if (query == null)
        {
            return "No results found";
        }

        var statistics = await Connection.QueryAsync<ApartmentBedroomsAveragesDto>(query, new { NumOfBedrooms = numOfBedrooms });
        return statistics.ToList();
    }
}
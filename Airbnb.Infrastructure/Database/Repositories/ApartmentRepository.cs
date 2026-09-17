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
        var apartmentsQuery = DbSet.AsQueryable();

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
        var apartment = await Connection.QueryAsync<Apartment>(Queries.UpsertApartment, apartmentToUpsert, Transaction);
        if (!apartment.Any())
        {
            return "Could not upsert the apartment";
        }

        return apartment.First();
    }

    public async Task<Result<List<ApartmentByProfitDto>>> GetTopApartmentsByProfitAsync(int numOfApartments, string userId)
    {
        var apartments = await Connection.QueryAsync<ApartmentByProfitDto>(Queries.GetTopApartmentsByProfit, new { Count = numOfApartments, UserId = userId }, Transaction);
        return apartments.ToList();
    }

    public async Task<Result<List<ApartmentCityAveragesDto>>> GetAverageApartmentCountAndPricePerCityAsync(string city)
    {
        var statistics = await Connection.QueryAsync<ApartmentCityAveragesDto>(Queries.GetAverageApartmentCountAndPricePerCity, new { City = city }, Transaction);
        return statistics.ToList();
    }

    public async Task<Result<List<ApartmentTypeAveragesDto>>> GetAverageApartmentPricePerTypeAsync(ApartmentType type)
    {
        var statistics = await Connection.QueryAsync<ApartmentTypeAveragesDto>(Queries.GetAverageApartmentPricePerType, new { Type = type }, Transaction);
        return statistics.ToList();
    }

    public async Task<Result<List<ApartmentTypeBookingStatsDto>>> GetApartmentBookingCountAndAverageStayByTypeAsync(ApartmentType type)
    {
        var statistics = await Connection.QueryAsync<ApartmentTypeBookingStatsDto>(Queries.GetApartmentBookingCountAndAverageStayByType, new { Type = type }, Transaction);
        return statistics.ToList();
    }

    public async Task<Result<List<ApartmentBedroomsAveragesDto>>> GetApartmentCountAndAveragePriceByBedroomsAsync(int numOfBedrooms)
    {
        var statistics = await Connection.QueryAsync<ApartmentBedroomsAveragesDto>(Queries.GetApartmentCountAndAveragePriceByBedrooms, new { NumOfBedrooms = numOfBedrooms }, Transaction);
        return statistics.ToList();
    }
}
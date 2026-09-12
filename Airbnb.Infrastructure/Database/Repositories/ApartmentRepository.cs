using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;
using Dapper;
using Microsoft.Data.SqlClient;
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

    public async Task<List<Apartment>> GetTopApartmentsByProfitAsync(int numOfApartments)
    {
        await using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
        
        string query = $"""
                       SELECT TOP({numOfApartments}) 
                            a.Title AS ApartmentTitle, 
                            COUNT(*) AS NumberOfBookings, 
                            SUM(BookedTotalPrice) AS TotalProfit
                       FROM Apartments a
                       LEFT JOIN Bookings b ON a.Id = b.ApartmentId
                       GROUP BY a.Id, a.Title
                       ORDER BY TotalProfit DESC
                       """;
        var apartments = await connection.QueryAsync(query);
        
        return new List<Apartment>();
    }
}
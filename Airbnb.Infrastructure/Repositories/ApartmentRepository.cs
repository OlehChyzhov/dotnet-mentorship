using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Enums;
using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class ApartmentRepository : Repository<Apartment, Guid>, IApartmentRepository
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
}
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IApartmentRepository : IRepository<Apartment>
{
    Task<PagedList<Apartment>> GetApartmentsPagedAsync(ApartmentQuery query);
}
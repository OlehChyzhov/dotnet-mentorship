using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;
using Airbnb.Domain.Requests;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IApartmentRepository : IRepository<Apartment>
{
    Task<PagedList<Apartment>> GetApartmentsPagedAsync(ApartmentParameters parameters);
}
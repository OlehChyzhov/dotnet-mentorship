using Airbnb.Domain.Models;
using Airbnb.Domain.Requests;
using Airbnb.Domain.Requests.Paging;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IApartmentRepository : IRepository<Apartment>
{
    Task<PagedList<Apartment>> GetApartmentsPagedAsync(ApartmentParameters parameters);
}
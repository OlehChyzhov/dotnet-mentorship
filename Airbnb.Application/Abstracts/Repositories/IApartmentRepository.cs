using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;

namespace Airbnb.Application.Abstracts.Repositories;

public interface IApartmentRepository : IRepository<Domain.Models.Apartment>
{
    Task<PagedList<Domain.Models.Apartment>> GetApartmentsPagedAsync(ApartmentPagingParamters query);
}
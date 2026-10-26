using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.HTTP;

namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IApartmentAuditRepository : IRepository<ApartmentAuditEntity>
{
    Task<PagedResult<ApartmentAuditEntity>> GetApartmentsPagedAsync(ApartmentAuditFilter filter);
}
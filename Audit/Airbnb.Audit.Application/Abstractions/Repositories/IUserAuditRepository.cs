using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.HTTP;

namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IUserAuditRepository : IRepository<UserAuditEntity>
{
    public Task<PagedResult<UserAuditEntity>> GetAllAuditsPaged(UserAuditFilter parameters);
}
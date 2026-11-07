using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Shared.Common;

namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IUserAuditRepository : IRepository<UserAuditEntity>
{
    public Task<PagedResult<UserAuditEntity>> GetAllAuditsPaged(UserAuditFilter parameters);
}
using Airbnb.Audit.Application.Querying;
using Airbnb.Audit.Domain.Models;
using Airbnb.Contracts;
using Airbnb.Contracts.Paging;

namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IUserAuditRepository : IRepository<UserAuditChangeEntity>
{
    public Task<PagedList<UserAuditChangeEntity>> GetAllAuditsPaged(UserAuditPagingParameters parameters);
}
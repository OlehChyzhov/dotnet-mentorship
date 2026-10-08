using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Application.Querying;
using Airbnb.Contracts;
using Airbnb.Contracts.Paging;

namespace Airbnb.Audit.Application.Abstractions.Services;

public interface IUserAuditService
{
    public Task<Result<UserAuditDto>> GetAuditByIdAsync(string id);

    Task<Result<PagedList<UserAuditDto>>> GetAuditsPaged(UserAuditPagingParameters parameters);
}
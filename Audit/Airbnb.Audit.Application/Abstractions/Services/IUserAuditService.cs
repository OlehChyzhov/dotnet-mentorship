using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Application.Querying;
using Airbnb.Shared.HTTP;

namespace Airbnb.Audit.Application.Abstractions.Services;

public interface IUserAuditService
{
    public Task<Result<UserAuditDto>> GetAuditByIdAsync(string id);

    Task<Result<PagedResult<UserAuditDto>>> GetAuditsPaged(UserAuditFilter parameters);
}
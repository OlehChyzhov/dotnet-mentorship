using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Application.Querying;
using Airbnb.Contracts.HTTP;
using Airbnb.Contracts.HTTP.Paging;

namespace Airbnb.Audit.Application.Abstractions.Services;

public interface IApartmentAuditService
{
    public Task<Result<ApartmentAuditDto>> GetAuditByIdAsync(string id);

    Task<Result<PagedResult<ApartmentAuditDto>>> GetAuditsPaged(ApartmentAuditFilter parameters);
}
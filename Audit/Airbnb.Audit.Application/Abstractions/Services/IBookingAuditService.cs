using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Application.Querying;
using Airbnb.Shared.HTTP;

namespace Airbnb.Audit.Application.Abstractions.Services;

public interface IBookingAuditService
{
    public Task<Result<BookingAuditDto>> GetAuditByIdAsync(string id);

    Task<Result<PagedResult<BookingAuditDto>>> GetAuditsPaged(BookingAuditFilter parameters);
}
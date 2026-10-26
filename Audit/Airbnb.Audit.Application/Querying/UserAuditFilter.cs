using Airbnb.Shared.HTTP;

namespace Airbnb.Audit.Application.Querying;

public record UserAuditFilter : PagingPramsBase
{
    public string? UserId { get; set; }
    public DateTime? Date { get; set; }
}
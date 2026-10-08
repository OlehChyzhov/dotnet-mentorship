using Airbnb.Contracts.Paging;

namespace Airbnb.Audit.Application.Querying;

public record UserAuditPagingParameters : PagingParametersBase
{
    public string? UserId { get; set; }
    public DateTime? Date { get; set; }
}
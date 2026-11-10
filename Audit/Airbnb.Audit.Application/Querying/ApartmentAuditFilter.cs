using Airbnb.Shared.Common.Paging;

namespace Airbnb.Audit.Application.Querying;

public record ApartmentAuditFilter : PagingPramsBase
{
    public string? ApartmentId { get; set; }
    public DateTime? Date { get; set; }
}
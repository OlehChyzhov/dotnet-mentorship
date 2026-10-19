using Airbnb.Contracts.HTTP.Paging;

namespace Airbnb.Audit.Application.Querying;

public record ApartmentAuditFilter : PagingPramsBase
{
    public string? ApartmentId { get; set; }
    public DateTime? Date { get; set; }
}
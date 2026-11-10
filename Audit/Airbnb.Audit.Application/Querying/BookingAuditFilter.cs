using Airbnb.Shared.Common.Paging;

namespace Airbnb.Audit.Application.Querying;

public record BookingAuditFilter : PagingPramsBase
{
    public string? BookingId { get; set; }
    public DateTime? Date { get; set; }
}
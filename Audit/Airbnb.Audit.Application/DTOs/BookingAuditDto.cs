using Airbnb.Audit.Domain.Enums;

namespace Airbnb.Audit.Application.DTOs;

public class BookingAuditDto
{
    public string AuditId { get; set; } = string.Empty;
    public string BookingId { get; set; } = string.Empty;
    public ChangeType ChangeType { get; set; }
    public string? UpdatedProperty { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime EventOccuredAt { get; set; }
    public DateTime AuditCreatedAt { get; set; }
}
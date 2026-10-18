using Airbnb.Audit.Domain.Enums;

namespace Airbnb.Audit.Application.DTOs;

public class ApartmentAuditDto
{
    public string Id { get; set; } = string.Empty;
    public string ApartmentId { get; set; } = string.Empty;
    public ChangeType ChangeType { get; set; }
    public string? UpdatedProperty { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime EventOccuredAt { get; set; }
    public DateTime AuditCreatedAt { get; set; }
}
using Airbnb.Audit.Domain.Attributes;
using Airbnb.Audit.Domain.Enums;

namespace Airbnb.Audit.Domain.Models;

[BsonCollection("booking_audits")]
public class BookingAuditEntity
{
    public string? Id { get; set; }
    public string? BookingId { get; set; }
    public ChangeType? ChangeType { get; set; }
    public string? UpdatedProperty { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
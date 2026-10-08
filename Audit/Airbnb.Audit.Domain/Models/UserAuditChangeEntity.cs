using Airbnb.Audit.Domain.Enums;

namespace Airbnb.Audit.Domain.Models;

public class UserAuditChangeEntity
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public ChangeType ChangeType { get; set; }
    public string? OldValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
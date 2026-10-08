using Airbnb.Audit.Domain.Enums;

namespace Airbnb.Audit.Application.DTOs;

public record UserAuditDto
{
    public string? Id { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public ChangeType ChangeType { get; init; }
    public string? OldValue { get; init; }
    public DateTime CreatedAt { get; init; }
}
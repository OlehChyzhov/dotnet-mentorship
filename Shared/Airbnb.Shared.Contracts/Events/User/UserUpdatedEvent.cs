namespace Airbnb.Shared.Contracts.Events.User;

public record UserUpdatedEvent
{
    public string? UserId { get; set; } = string.Empty;
    public string? PropertyName { get; set; } = string.Empty;
    public string? OldValue { get; set; } = string.Empty;
    public string? NewValue { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } =  DateTime.UtcNow;
}
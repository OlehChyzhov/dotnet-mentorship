namespace Airbnb.Contracts.Broker.Messages.User;

public record UserUpdated
{
    public string? EntityId { get; set; } = string.Empty;
    public string? PropertyName { get; set; } = string.Empty;
    public string? OldValue { get; set; } = string.Empty;
    public string? NewValue { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } =  DateTime.UtcNow;
}
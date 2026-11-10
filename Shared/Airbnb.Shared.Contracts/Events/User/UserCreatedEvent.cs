namespace Airbnb.Shared.Contracts.Events.User;

public record UserCreatedEvent
{
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}
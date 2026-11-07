namespace Airbnb.Shared.Contracts.Events.User;

public record UserDeletedEvent
{
    public string UserId { get; set; } =  string.Empty;
    public DateTime DeletedAt { get; set; } =  DateTime.UtcNow;
}
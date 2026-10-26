namespace Airbnb.Shared.Messages.User;

public record UserCreated
{
    public string EntityId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}
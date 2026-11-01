namespace Airbnb.Shared.Contracts.Messages.User;

public record UserDeleted
{
    public string EntityId { get; set; } =  string.Empty;
    public DateTime DeletedAt { get; set; } =  DateTime.UtcNow;
}
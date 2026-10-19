namespace Airbnb.Contracts.Broker.Messages.User;

public record UserDeleted
{
    public string EntityId { get; set; } =  string.Empty;
    public DateTime DeletedAt { get; set; } =  DateTime.UtcNow;
}
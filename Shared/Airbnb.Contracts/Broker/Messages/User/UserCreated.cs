namespace Airbnb.Contracts.Broker.Messages.User;

public record UserCreated
{
    public string EntityId { get; set; } = string.Empty;
}
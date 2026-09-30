namespace Airbnb.Domain.Messages;

public abstract record UserMessageBase
{
    public required string Id { get; init; }
    public string? Name { get; init; }
    public required DateTime OccuredOn { get; init; }
}
public record UserCreatedMessage : UserMessageBase
{
    public required string? Email { get; init; }
}

public record UserEmailChangedMessage : UserMessageBase
{
    public required string NewEmail { get; init; }
    public string? OldEmail { get; init; }
}

public record UserDeletedMessage : UserMessageBase
{
    public string? Email { get; init; }
}
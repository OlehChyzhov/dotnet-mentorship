namespace Airbnb.Messaging.Options;

public class MessageBrokerOptions
{
    public string HostUri { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
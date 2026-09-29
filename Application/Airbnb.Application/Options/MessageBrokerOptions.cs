namespace Airbnb.Application.Options;

public class MessageBrokerOptions
{
    public string HostName { get; init; } = string.Empty;
    public int Port { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Exchange { get; init; } = string.Empty;
    public string Queue { get; init; } = string.Empty;
    public string ClientProvidedName { get; init; } = string.Empty;
}
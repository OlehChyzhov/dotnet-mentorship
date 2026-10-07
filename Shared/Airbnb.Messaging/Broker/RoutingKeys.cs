using Airbnb.Messaging.Messages;

namespace Airbnb.Messaging.Broker;

public static class RoutingKeys
{
    private static readonly Dictionary<Type, string> Map = new()
    {
        [typeof(UserCreated)] = "user.created",
        [typeof(UserDeleted)] = "user.deleted",
        [typeof(UserEmailChanged)] = "user.email_changed",
    };
    
    public static string For<T>() => Map[typeof(T)];
}
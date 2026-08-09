using System.Text.Json.Serialization;

namespace Airbnb.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    Client,
    Host
}
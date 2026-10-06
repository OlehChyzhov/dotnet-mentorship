using System.Text.Json.Serialization;

namespace Airbnb.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}
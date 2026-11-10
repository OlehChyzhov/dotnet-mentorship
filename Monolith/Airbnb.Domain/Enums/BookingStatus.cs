using System.Text.Json.Serialization;

namespace Airbnb.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    AwaitingPayment = 1,
    PaymentExpired = 2,
    PaymentSucceeded = 3,
    PaymentFailed = 4,
}
using System.Text.Json.Serialization;

namespace Airbnb.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    AwaitingPayment = 1,
    Paid = 2,
    PaymentFailed = 3,
}
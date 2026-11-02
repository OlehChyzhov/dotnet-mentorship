namespace Airbnb.Shared.Contracts.Events.Payment;

public class PaymentSessionCreationFailedEvent
{
    public string FailureReason { get; set; } = string.Empty;
}
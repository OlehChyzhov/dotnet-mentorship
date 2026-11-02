namespace Airbnb.Shared.Contracts.Events.Payment;

public class PaymentSessionCreationSucceededEvent
{
    public string PaymentSessionUrl { get; set; } = string.Empty;
}
namespace Airbnb.Payment.Domain;

public enum PaymentTransactionStatus { Pending, Succeeded, Failed, Expired }

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentTransactionStatus Status { get; set; }
    public string StripeSessionId { get; set; } = string.Empty;
    public string StripePaymentIntendId { get; set; } = string.Empty;
}
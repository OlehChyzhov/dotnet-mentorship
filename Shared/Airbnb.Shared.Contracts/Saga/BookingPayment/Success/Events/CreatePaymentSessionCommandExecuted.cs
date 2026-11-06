namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;

public class CreatePaymentSessionCommandExecuted
{
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? PaymentSessionId { get; set; }
    public string? PaymentSessionUrl { get; set; }
    public bool IsSuccessful { get; set; }
}
using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;

namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Commands;

public class FinishPaymentTransactionCommand
{ 
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}
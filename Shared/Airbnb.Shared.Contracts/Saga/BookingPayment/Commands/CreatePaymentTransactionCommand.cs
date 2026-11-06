namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Commands;

public class CreatePaymentTransactionCommand
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
}
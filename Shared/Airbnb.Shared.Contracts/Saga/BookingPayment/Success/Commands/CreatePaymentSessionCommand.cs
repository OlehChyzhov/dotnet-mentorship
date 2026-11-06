namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Commands;

public class CreatePaymentSessionCommand
{
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public decimal PaymentAmount { get; set; }

    public string ApartmentName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
}
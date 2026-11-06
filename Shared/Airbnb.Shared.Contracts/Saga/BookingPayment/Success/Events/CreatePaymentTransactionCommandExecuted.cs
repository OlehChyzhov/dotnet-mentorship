namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;

public class CreatePaymentTransactionCommandExecuted
{
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public  decimal PaymentAmount { get; set; }
    public string ApartmentName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
    public bool IsSuccessful { get; set; }
}
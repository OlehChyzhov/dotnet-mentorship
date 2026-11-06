namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Commands;

public class CreatePaymentTransactionCommand
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    
    public string ApartmentName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
}
namespace Airbnb.Shared.Contracts.Messages;

public class CreatePaymentSessionMessage
{
    public Guid BookingId { get; set; }
    public string ApartmentName { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestsCount { get; set; }
    public double PaymentAmount { get; set; }
}
namespace Airbnb.Shared.Contracts.Commands.Payment;

public class CreatePaymentSessionCommand
{
    public Guid BookingId { get; set; }
    public string ApartmentName { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestsCount { get; set; }
    public double PaymentAmount { get; set; }
}
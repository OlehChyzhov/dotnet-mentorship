namespace Airbnb.Shared.Contracts.Commands.Payment;

public class CreatePaymentSessionCommand
{
    public Guid BookingId { get; set; }
    public double PaymentAmount { get; set; }
}
namespace Airbnb.Payment.Application.DTOs;

public class PaymentSessionCreationArguments
{
    public Guid BookingId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
}
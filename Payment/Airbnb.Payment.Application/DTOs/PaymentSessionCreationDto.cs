namespace Airbnb.Payment.Application.DTOs;

public class PaymentSessionCreationDto
{
    public string PaymentSessionUrl { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}
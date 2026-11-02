using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.DTOs;
using Airbnb.Payment.Application.Options;
using Airbnb.Shared.Common;
using Airbnb.Shared.Contracts.Commands.Payment;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Airbnb.Payment.Application.Services;

public class StripePaymentService : IPaymentService
{
    private readonly StripeConfig _stripeConfig;
    private readonly SessionService _sessionService;

    public StripePaymentService(IOptions<StripeConfig> stripeConfig, SessionService sessionService)
    {
        _stripeConfig = stripeConfig.Value;
        _sessionService = sessionService;
    }
    
    public Result<PaymentSessionCreationDto> CreatePaymentSession(CreatePaymentSessionCommand command)
    {
        try
        {
            StripeConfiguration.ApiKey = _stripeConfig.SecretKey;

            var options = new SessionCreateOptions()
            {
                AllowedPaymentMethodTypes = new List<string>() { "card" },
                LineItems = new List<SessionLineItemOptions>()
                {
                    new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(command.PaymentAmount * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = $"Booking of the '{command.ApartmentName}'",
                                Description = $"{command.CheckIn.Date} - {command.CheckOut.Date} for {command.GuestsCount} guests",
                            }
                        },
                        Quantity = 1,
                    }
                },
                Mode = "payment",
                ClientReferenceId = command.BookingId.ToString(),
                Metadata = new Dictionary<string, string>()
                {
                    ["bookingId"] = command.BookingId.ToString(),
                },
                ExpiresAt = DateTime.UtcNow.AddMinutes(31),
                SuccessUrl = "https://example.com/success"
            };
        
            Session session = _sessionService.Create(options);

            var result = new PaymentSessionCreationDto()
            {
                PaymentSessionUrl = session.Url,
                SessionId = session.Id
            };

            return result;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
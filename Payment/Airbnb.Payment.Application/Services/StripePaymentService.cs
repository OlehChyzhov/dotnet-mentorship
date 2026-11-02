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
                                Name = $"Booking of the '{command.ApartmentName}'"
                            }
                        },
                        Quantity = 1,
                    }
                },
                Mode = "payment",
                SuccessUrl = _stripeConfig.SuccessUrl,
                CancelUrl = _stripeConfig.CancelUrl
            };
        
            Session session = _sessionService.Create(options);

            var result = new PaymentSessionCreationDto()
            {
                PaymentSessionUrl = session.Url
            };

            return result;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
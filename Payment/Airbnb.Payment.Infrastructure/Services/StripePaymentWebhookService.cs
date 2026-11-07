using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Infrastructure.Options;
using Airbnb.Shared.Common;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Event = Stripe.Event;

namespace Airbnb.Payment.Infrastructure.Services;

public class StripePaymentWebhookService : IPaymentWebhookService
{
    private readonly ILogger<StripePaymentWebhookService> _logger;
    private readonly StripeConfig _stripeConfig;
    private readonly IPublishEndpoint _publishEndpoint;

    public StripePaymentWebhookService(
        ILogger<StripePaymentWebhookService> logger,
        IOptions<StripeConfig> stripeConfig,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _stripeConfig = stripeConfig.Value;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> HandleWebhookAsync(string json, string signature)
    {
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _stripeConfig.WebhookSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "Invalid Stripe webhook signature");
            return "Invalid Stripe webhook signature";
        }

        var session = (Session)stripeEvent.Data.Object;
        
        if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
        {
            if (session.PaymentStatus == "paid")
            {
                await _publishEndpoint.Publish(new PaymentWebhookTriggeredEvent
                {
                    BookingId = Guid.Parse(session.ClientReferenceId),
                    PaymentIntentId = session.PaymentIntentId,
                    PaymentSessionId = session.Id,
                    IsSuccessful = true,
                    Status = PaymentStatus.Paid
                });
                
                _logger.LogInformation("Payment successful for booking with id {BookingId}", session.ClientReferenceId);
            }
        }
        else if (stripeEvent.Type == EventTypes.CheckoutSessionExpired)
        {
            await _publishEndpoint.Publish(new PaymentWebhookTriggeredEvent
            {
                BookingId = Guid.Parse(session.ClientReferenceId),
                PaymentIntentId = session.PaymentIntentId,
                PaymentSessionId = session.Id,
                IsSuccessful = true,
                Status = PaymentStatus.Expired
            });
            
            _logger.LogInformation("Payment expired for booking with id {BookingId}", session.ClientReferenceId);
        }
        else
        {
            await _publishEndpoint.Publish(new PaymentWebhookTriggeredEvent
            {
                BookingId = Guid.Parse(session.ClientReferenceId),
                PaymentIntentId = session.PaymentIntentId,
                PaymentSessionId = session.Id,
                IsSuccessful = false,
            });
            
            _logger.LogInformation("Unhandled Stripe event {Type}", stripeEvent.Type);
        }

        return true;
    }
}
using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.Options;
using Airbnb.Shared.Common;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Event = Stripe.Event;

namespace Airbnb.Payment.Application.Services;

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

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
            {
                var session = (Session)stripeEvent.Data.Object;
                if (session.PaymentStatus != "paid") break;

                await _publishEndpoint.Publish(new PaymentWebhookTriggeredEvent
                {
                    BookingId = Guid.Parse(session.ClientReferenceId),
                    Success = true,
                    PaymentIntentId = session.PaymentIntentId,
                    PaymentSessionId = session.Id
                });
                break;
            }
            case EventTypes.CheckoutSessionExpired:
            {
                var session = (Session)stripeEvent.Data.Object;

                await _publishEndpoint.Publish(new PaymentWebhookTriggeredEvent
                {
                    PaymentSessionId = session.Id,
                    BookingId = Guid.Parse(session.ClientReferenceId),
                    Success = false,
                    FailureReason = "Checkout session expired"
                });
                break;
            }
            default:
                _logger.LogInformation("Unhandled Stripe event {Type}", stripeEvent.Type);
                break;
        }

        return true;
    }
}
using Airbnb.Payment.Application.Options;
using Airbnb.Shared.Contracts.Events.Payment;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Event = Stripe.Event;

namespace Airbnb.Payment.API.Controllers;

[Route("payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IOptions<StripeConfig> _stripeConfig;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public PaymentController(
        ILogger<PaymentController> logger,
        IOptions<StripeConfig> stripeConfig,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _stripeConfig = stripeConfig;
        _publishEndpoint = publishEndpoint;
    }
    
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        Event stripeEvent;
        
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeConfig.Value.WebhookSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "Invalid Stripe webhook signature");
            return BadRequest();
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
            {
                var session = (Session)stripeEvent.Data.Object;
                if (session.PaymentStatus != "paid") break;

                await _publishEndpoint.Publish(new PaymentSucceededEvent
                {
                    BookingId = Guid.Parse(session.ClientReferenceId),
                    PaymentIntentId = session.PaymentIntentId,
                    AmountTotal = session.AmountTotal ?? 0
                });
                break;
            }
            case EventTypes.CheckoutSessionExpired:
            {
                var session = (Session)stripeEvent.Data.Object;

                await _publishEndpoint.Publish(new PaymentFailedEvent
                {
                    BookingId = Guid.Parse(session.ClientReferenceId),
                    Reason = "Checkout session expired"
                });
                break;
            }
            default:
                _logger.LogInformation("Unhandled Stripe event {Type}", stripeEvent.Type);
                break;
        }
        
        return Ok();
    }
}
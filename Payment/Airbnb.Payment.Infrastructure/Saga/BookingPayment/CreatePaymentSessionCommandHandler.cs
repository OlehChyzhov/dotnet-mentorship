using Airbnb.Payment.Infrastructure.Options;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Commands;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment;

public class CreatePaymentSessionCommandHandler : IConsumer<CreatePaymentSessionCommand>
{
    private readonly StripeConfig _stripeConfig;
    private readonly SessionService _sessionService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CreatePaymentSessionCommandHandler> _logger;
    
    public CreatePaymentSessionCommandHandler(
        IOptions<StripeConfig> stripeConfig,
        SessionService sessionService,
        IPublishEndpoint publishEndpoint,
        ILogger<CreatePaymentSessionCommandHandler> logger)
    {
        _stripeConfig = stripeConfig.Value;
        _sessionService = sessionService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<CreatePaymentSessionCommand> context)
    {
        var title = $"Booking of the apartment '{context.Message.ApartmentName}'";
        var description = $"From '{context.Message.CheckIn}' to '{context.Message.CheckOut}' for '{context.Message.GuestCount}' guests";

        try
        {
            var options = CreateSessionCreateOptions(context.Message.BookingId, title, description, context.Message.PaymentAmount);
            Session session = await _sessionService.CreateAsync(options);
            
            await _publishEndpoint.Publish(new CreatePaymentSessionCommandExecuted()
            {
                BookingId = context.Message.BookingId,
                PaymentTransactionId = context.Message.PaymentTransactionId,
                PaymentIntentId = session.PaymentIntentId,
                PaymentSessionId = session.Id,
                PaymentSessionUrl = session.Url,
                IsSuccessful = true
            });
        }
        catch (StripeException)
        {
            await _publishEndpoint.Publish(new CreatePaymentSessionCommandExecuted()
            {
                BookingId = context.Message.BookingId,
                PaymentTransactionId = context.Message.PaymentTransactionId,
                IsSuccessful = false
            });

            throw;
        }
    }

    private SessionCreateOptions CreateSessionCreateOptions(Guid bookingId, string title, string description, decimal amount)
    {
        return new SessionCreateOptions()
        {
            AllowedPaymentMethodTypes = new List<string>() { "card" },
            LineItems = new List<SessionLineItemOptions>()
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)Math.Round(amount * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = title,
                            Description = description,
                        }
                    },
                    Quantity = 1,
                }
            },
            Mode = "payment",
            ClientReferenceId = bookingId.ToString(),
            Metadata = new Dictionary<string, string>()
            {
                ["bookingId"] = bookingId.ToString(),
            },
            SuccessUrl = _stripeConfig.SuccessUrl,
            CancelUrl = _stripeConfig.CancelUrl
        };
    }
}
using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Options;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Success;

public class CreatePaymentSessionCommandHandler : IConsumer<CreatePaymentSessionCommand>
{
    private readonly StripeConfig _stripeConfig;
    private readonly SessionService _sessionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CreatePaymentSessionCommandHandler> _logger;

    public CreatePaymentSessionCommandHandler(
        IOptions<StripeConfig> stripeConfig,
        SessionService sessionService,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<CreatePaymentSessionCommandHandler> logger)
    {
        _stripeConfig = stripeConfig.Value;
        _sessionService = sessionService;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreatePaymentSessionCommand> context)
    {
        var transaction = await _unitOfWork.PaymentTransactions.GetByBookingIdAsync(context.Message.BookingId)
            ?? throw new InvalidOperationException($"Payment transaction for booking {context.Message.BookingId} was not found");

        if (string.IsNullOrEmpty(transaction.StripeSessionId))
        {
            var title = $"Booking of the apartment '{context.Message.ApartmentName}'";
            var description = $"From '{context.Message.CheckIn:d}' to '{context.Message.CheckOut:d}' for '{context.Message.GuestCount}' guests";

            var options = CreateSessionCreateOptions(context.Message.BookingId, title, description, context.Message.PaymentAmount);
            Session session = await _sessionService.CreateAsync(options, cancellationToken: context.CancellationToken);

            transaction.StripeSessionId = session.Id;
            transaction.StripePaymentSessionUrl = session.Url;
            transaction.PaymentSessionStatus = PaymentSessionStatus.Valid;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created payment session {SessionId} for booking {BookingId}", session.Id, context.Message.BookingId);
        }
        else
        {
            _logger.LogInformation("Payment session for booking {BookingId} already exists", context.Message.BookingId);
        }

        await _publishEndpoint.Publish(new CreatePaymentSessionCommandExecuted()
        {
            BookingId = context.Message.BookingId,
            PaymentTransactionId = transaction.Id,
            PaymentSessionId = transaction.StripeSessionId,
            PaymentSessionUrl = transaction.StripePaymentSessionUrl
        });
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

using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Application.Abstractions.Services;
using Airbnb.Payment.Domain;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public StripePaymentWebhookService(
        ILogger<StripePaymentWebhookService> logger,
        IOptions<StripeConfig> stripeConfig,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _stripeConfig = stripeConfig.Value;
        _unitOfWork = unitOfWork;
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

        if (stripeEvent.Type != EventTypes.CheckoutSessionCompleted
            && stripeEvent.Type != EventTypes.CheckoutSessionExpired)
        {
            _logger.LogInformation("Ignored Stripe event {Type}", stripeEvent.Type);
            return true;
        }

        if (stripeEvent.Data.Object is not Session session)
        {
            _logger.LogWarning("Stripe event {Type} does not contain a checkout session", stripeEvent.Type);
            return true;
        }

        if (!Guid.TryParse(session.ClientReferenceId, out var bookingId))
        {
            _logger.LogWarning("Checkout session {SessionId} has no valid booking id", session.Id);
            return true;
        }

        var transaction = await _unitOfWork.PaymentTransactions.GetByBookingIdAsync(bookingId);
        if (transaction == null || transaction.StripeSessionId != session.Id)
        {
            _logger.LogWarning("No payment transaction matches session {SessionId} for booking {BookingId}", session.Id, bookingId);
            return true;
        }

        if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
        {
            await HandleSessionCompletedAsync(session, transaction);
        }
        else
        {
            await HandleSessionExpiredAsync(session, transaction);
        }

        return true;
    }

    private async Task HandleSessionCompletedAsync(Session session, PaymentTransaction transaction)
    {
        if (session.PaymentStatus != "paid")
        {
            _logger.LogWarning("Checkout session {SessionId} completed with payment status {PaymentStatus}", session.Id, session.PaymentStatus);
            return;
        }

        if (!string.IsNullOrEmpty(transaction.StripePaymentIntendId))
        {
            _logger.LogInformation("Duplicate completed webhook for booking {BookingId}", transaction.BookingId);
            return;
        }

        transaction.StripePaymentIntendId = session.PaymentIntentId;
        transaction.RefundStatus = RefundStatus.RefundPending;
        await _unitOfWork.SaveChangesAsync();

        await _publishEndpoint.Publish(new PaymentWebhookTriggeredEvent
        {
            BookingId = transaction.BookingId,
            PaymentIntentId = session.PaymentIntentId,
            PaymentSessionId = session.Id,
            Status = PaymentStatus.Paid
        });

        _logger.LogInformation("Payment successful for booking with id {BookingId}", transaction.BookingId);
    }

    private async Task HandleSessionExpiredAsync(Session session, PaymentTransaction transaction)
    {
        if (transaction.PaymentSessionStatus == PaymentSessionStatus.Invalid)
        {
            _logger.LogInformation("Ignored expired webhook for already invalid session of booking {BookingId}", transaction.BookingId);
            return;
        }

        transaction.PaymentSessionStatus = PaymentSessionStatus.Invalid;
        await _unitOfWork.SaveChangesAsync();

        await _publishEndpoint.Publish(new PaymentWebhookTriggeredEvent
        {
            BookingId = transaction.BookingId,
            PaymentSessionId = session.Id,
            Status = PaymentStatus.Expired
        });

        _logger.LogInformation("Payment expired for booking with id {BookingId}", transaction.BookingId);
    }
}

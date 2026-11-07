using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class RefundPaidAmountCommandHandler(
    IUnitOfWork unitOfWork,
    RefundService refundService,
    IPublishEndpoint publishEndpoint,
    ILogger<RefundPaidAmountCommandHandler> logger) : IConsumer<RefundPaidAmountCommand>
{
    public async Task Consume(ConsumeContext<RefundPaidAmountCommand> context)
    {
        var transaction = await unitOfWork.PaymentTransactions.GetByBookingIdAsync(context.Message.BookingId);

        if (transaction != null && transaction.RefundStatus == RefundStatus.RefundPending)
        {
            var refundOptions = CreateRefundOptions(transaction.StripePaymentIntendId, transaction.PaymentAmount, transaction.BookingId);

            var requestOptions = new RequestOptions { IdempotencyKey = $"refund-{transaction.Id}" };

            await refundService.CreateAsync(refundOptions, requestOptions, context.CancellationToken);

            transaction.RefundStatus = RefundStatus.RefundSucceeded;
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Refunded {Amount} for booking {BookingId}", transaction.PaymentAmount, transaction.BookingId);
        }

        await publishEndpoint.Publish(new RefundingPaidAmountCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
        });
    }

    private static RefundCreateOptions CreateRefundOptions(string paymentIntendId, decimal refundAmount, Guid bookingId)
    {
        return new RefundCreateOptions()
        {
            PaymentIntent = paymentIntendId,
            Amount = (long)Math.Round(refundAmount * 100),
            Metadata = new Dictionary<string, string>()
            {
                ["bookingId"] = bookingId.ToString(),
            },
        };
    }
}

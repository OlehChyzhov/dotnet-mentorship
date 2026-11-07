using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Stripe;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class RefundPaidAmountCommandHandler(ApplicationDbContext dbContext, RefundService refundService, IPublishEndpoint publishEndpoint) : IConsumer<RefundPaidAmountCommand>
{
    public async Task Consume(ConsumeContext<RefundPaidAmountCommand> context)
    {
        var transaction = dbContext.PaymentTransactions.First(transaction => transaction.Id == context.Message.TransactionId);

        if (transaction.RefundStatus == RefundStatus.RefundPending)
        {
            var refundOptions = CreateRefundOptions(transaction.StripePaymentIntendId, transaction.PaymentAmount, transaction.BookingId);
            await refundService.CreateAsync(refundOptions);
            await publishEndpoint.Publish(new RefundingPaidAmountCommandExecuted()
            {
                TransactionId = context.Message.TransactionId,
                BookingId = context.Message.BookingId,
            });
        }
    }

    private RefundCreateOptions CreateRefundOptions(string paymentIntendId, decimal refundAmount, Guid bookingId)
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
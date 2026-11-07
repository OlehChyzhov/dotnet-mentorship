using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class InvalidatePaymentSessionCommandHandler(ApplicationDbContext dbContext, SessionService sessionService, IPublishEndpoint publishEndpoint) : IConsumer<InvalidatePaymentSessionCommand>
{
    public async Task Consume(ConsumeContext<InvalidatePaymentSessionCommand> context)
    {
        var transaction = await dbContext.PaymentTransactions
            .FirstAsync(transaction => transaction.Id == context.Message.TransactionId);

        if (transaction.PaymentSessionStatus == PaymentSessionStatus.Valid)
        {
            transaction.PaymentSessionStatus = PaymentSessionStatus.Invalid;
            await dbContext.SaveChangesAsync();
        
            await sessionService.ExpireAsync(transaction.StripeSessionId);

            await publishEndpoint.Publish(new InvalidatePaymentSessionCommandExecuted()
            {
                TransactionId = context.Message.TransactionId,
                BookingId = context.Message.BookingId,
            });
        }
        else
        {
            await publishEndpoint.Publish(new InvalidatePaymentSessionCommandExecuted()
            {
                TransactionId = context.Message.TransactionId,
                BookingId = context.Message.BookingId,
            });
        }
    }
}
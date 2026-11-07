using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Stripe.Checkout;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class InvalidatePaymentSessionCommandHandler(IUnitOfWork unitOfWork, SessionService sessionService, IPublishEndpoint publishEndpoint) : IConsumer<InvalidatePaymentSessionCommand>
{
    public async Task Consume(ConsumeContext<InvalidatePaymentSessionCommand> context)
    {
        var transaction = await unitOfWork.PaymentTransactions.GetByBookingIdAsync(context.Message.BookingId);

        if (transaction != null)
        {
            if (!string.IsNullOrEmpty(transaction.StripeSessionId))
            {
                var session = await sessionService.GetAsync(transaction.StripeSessionId, cancellationToken: context.CancellationToken);
                if (session.Status == "open")
                {
                    await sessionService.ExpireAsync(transaction.StripeSessionId, cancellationToken: context.CancellationToken);
                }
            }

            if (transaction.PaymentSessionStatus != PaymentSessionStatus.Invalid)
            {
                transaction.PaymentSessionStatus = PaymentSessionStatus.Invalid;
                await unitOfWork.SaveChangesAsync();
            }
        }

        await publishEndpoint.Publish(new InvalidatePaymentSessionCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
        });
    }
}

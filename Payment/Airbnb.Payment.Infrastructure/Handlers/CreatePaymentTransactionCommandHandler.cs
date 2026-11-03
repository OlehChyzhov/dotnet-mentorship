using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Commands;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Events;
using MassTransit;

namespace Airbnb.Payment.Infrastructure.Handlers;

public class CreatePaymentTransactionCommandHandler(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IConsumer<CreatePaymentTransactionCommand>
{
    public async Task Consume(ConsumeContext<CreatePaymentTransactionCommand> context)
    {
        PaymentTransaction paymentTransaction;

        try
        {
            paymentTransaction = new PaymentTransaction()
            {
                Id = context.Message.BookingId,
                BookingId = context.Message.BookingId,
                Status = PaymentTransactionStatus.Pending,
                Amount = context.Message.Amount,
                StripeSessionId = string.Empty,
                StripePaymentIntendId = string.Empty,
            };

            dbContext.PaymentTransactions.Add(paymentTransaction);
            await dbContext.SaveChangesAsync(context.CancellationToken);
            
            await publishEndpoint.Publish(new PaymentTransactionCreatedEvent()
            {
                BookingId = paymentTransaction.BookingId,
                IsCreated = true,
            });
        }
        catch (Exception ex)
        {
            await publishEndpoint.Publish(new PaymentTransactionCreatedEvent()
            {
                BookingId = context.Message.BookingId,
                IsCreated = false,
            });
        }
    }
}
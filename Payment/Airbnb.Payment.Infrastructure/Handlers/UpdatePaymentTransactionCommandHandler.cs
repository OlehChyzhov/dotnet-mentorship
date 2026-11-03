using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Commands;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Events;
using MassTransit;

namespace Airbnb.Payment.Infrastructure.Handlers;

public class UpdatePaymentTransactionCommandHandler(ApplicationDbContext dbContext, IPublishEndpoint endpointPublisher) : IConsumer<UpdatePaymentTransactionCommand>
{
    public async Task Consume(ConsumeContext<UpdatePaymentTransactionCommand> context)
    {
        var paymentTransaction = dbContext.PaymentTransactions.FirstOrDefault(transaction => transaction.Id == context.Message.BookingId);
        
        if (paymentTransaction == null)
        {
            await endpointPublisher.Publish<PaymentTransactionUpdatedEvent>(new PaymentTransactionUpdatedEvent()
            {
                BookingId = context.Message.BookingId,
                IsFailed = true,
                IsExpired = false,
                IsSuccessful = false,
            });

            return;
        }
        
        if (context.Message.Success)
        {
            paymentTransaction.StripeSessionId = context.Message.PaymentSessionId!;
            paymentTransaction.StripePaymentIntendId = context.Message.PaymentIntentId!;
            paymentTransaction.Status = PaymentTransactionStatus.Succeeded;
        }
        else
        {
            paymentTransaction.Status = PaymentTransactionStatus.Expired;
        }
        
        dbContext.PaymentTransactions.Update(paymentTransaction);
        await dbContext.SaveChangesAsync();
        
        await endpointPublisher.Publish<PaymentTransactionUpdatedEvent>(new PaymentTransactionUpdatedEvent()
        {
            BookingId = context.Message.BookingId,
            IsSuccessful = paymentTransaction.Status == PaymentTransactionStatus.Succeeded,
            IsExpired = paymentTransaction.Status == PaymentTransactionStatus.Expired,
            IsFailed = false
        });
    }
}
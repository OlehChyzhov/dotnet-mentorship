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
            paymentTransaction = new PaymentTransaction()
            {
                Id = context.Message.BookingId,
                BookingId = context.Message.BookingId,
            };
            
            dbContext.PaymentTransactions.Add(paymentTransaction);
            await dbContext.SaveChangesAsync();
        }

        if (context.Message.Success)
        {
            paymentTransaction.StripeSessionId = context.Message.PaymentSessionId!;
            paymentTransaction.StripePaymentIntendId = context.Message.PaymentIntentId!;
            paymentTransaction.Status = PaymentTransactionStatus.Succeeded;
        }
        else
        {
            paymentTransaction.Status = PaymentTransactionStatus.Failed;
        }
        
        dbContext.PaymentTransactions.Update(paymentTransaction);
        await dbContext.SaveChangesAsync();
        
        await endpointPublisher.Publish<PaymentTransactionUpdatedEvent>(new PaymentTransactionUpdatedEvent()
        {
            BookingId = context.Message.BookingId,
            Success = paymentTransaction.Status == PaymentTransactionStatus.Succeeded ? true : false,
        });
    }
}
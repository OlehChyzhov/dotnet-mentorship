using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class RevertUpdatingTransactionAsSuccessCommandHandler(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IConsumer<RevertUpdatingTransactionAsSuccessCommand>
{
    public async Task Consume(ConsumeContext<RevertUpdatingTransactionAsSuccessCommand> context)
    {
        var transaction = dbContext.PaymentTransactions
            .First(transaction => transaction.Id == context.Message.TransactionId);

        transaction.TransactionStatus = PaymentTransactionStatus.Failed;
        await dbContext.SaveChangesAsync();

        await publishEndpoint.Publish(new RevertUpdatingTransactionAsSuccessCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
        });
    }
}
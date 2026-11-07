using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class RevertTransactionCreateCommandHandler(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IConsumer<RevertTransactionCreateCommand>
{
    public async Task Consume(ConsumeContext<RevertTransactionCreateCommand> context)
    {
        var transaction = await dbContext.PaymentTransactions
            .FirstAsync(transaction => transaction.Id == context.Message.TransactionId);

        transaction.TransactionStatus = PaymentTransactionStatus.Failed;
        await dbContext.SaveChangesAsync();

        await publishEndpoint.Publish(new RevertingTransactionCreateCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
        });
    }
}
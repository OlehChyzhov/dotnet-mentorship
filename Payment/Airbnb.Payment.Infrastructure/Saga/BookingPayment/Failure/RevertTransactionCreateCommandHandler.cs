using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class RevertTransactionCreateCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint) : IConsumer<RevertTransactionCreateCommand>
{
    public async Task Consume(ConsumeContext<RevertTransactionCreateCommand> context)
    {
        var transaction = await unitOfWork.PaymentTransactions.GetByBookingIdAsync(context.Message.BookingId);

        if (transaction != null && transaction.TransactionStatus != PaymentTransactionStatus.Failed)
        {
            transaction.TransactionStatus = PaymentTransactionStatus.Failed;
            await unitOfWork.SaveChangesAsync();
        }

        await publishEndpoint.Publish(new RevertingTransactionCreateCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
        });
    }
}

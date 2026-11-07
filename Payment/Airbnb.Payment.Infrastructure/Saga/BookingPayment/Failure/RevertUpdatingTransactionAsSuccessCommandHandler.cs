using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Failure;

public class RevertUpdatingTransactionAsSuccessCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint) : IConsumer<RevertUpdatingTransactionAsSuccessCommand>
{
    public async Task Consume(ConsumeContext<RevertUpdatingTransactionAsSuccessCommand> context)
    {
        var transaction = await unitOfWork.PaymentTransactions.GetByBookingIdAsync(context.Message.BookingId);

        if (transaction != null && transaction.TransactionStatus != PaymentTransactionStatus.Failed)
        {
            transaction.TransactionStatus = PaymentTransactionStatus.Failed;
            await unitOfWork.SaveChangesAsync();
        }

        await publishEndpoint.Publish(new RevertUpdatingTransactionAsSuccessCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
        });
    }
}

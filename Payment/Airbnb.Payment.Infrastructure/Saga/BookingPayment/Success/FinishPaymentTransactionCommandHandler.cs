using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Success;

public class FinishPaymentTransactionCommandHandler : IConsumer<UpdateTransactionAsSuccessCommand>
{
    private readonly ILogger<FinishPaymentTransactionCommandHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUnitOfWork _unitOfWork;

    public FinishPaymentTransactionCommandHandler(
        ILogger<FinishPaymentTransactionCommandHandler> logger,
        IPublishEndpoint publishEndpoint,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UpdateTransactionAsSuccessCommand> context)
    {
        var transaction = await _unitOfWork.PaymentTransactions.GetByBookingIdAsync(context.Message.BookingId)
            ?? throw new InvalidOperationException($"Payment transaction for booking {context.Message.BookingId} was not found");

        transaction.TransactionStatus = context.Message.PaymentStatus == PaymentStatus.Paid ? PaymentTransactionStatus.Succeeded : PaymentTransactionStatus.Expired;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Payment transaction for booking {BookingId} finished as {Status}", context.Message.BookingId, transaction.TransactionStatus);

        await _publishEndpoint.Publish(new UpdateTransactionAsSuccessCommandExecuted()
        {
            TransactionId = transaction.Id,
            BookingId = context.Message.BookingId,
        });
    }
}

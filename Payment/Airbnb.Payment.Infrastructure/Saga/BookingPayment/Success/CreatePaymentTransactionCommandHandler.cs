using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Success;

public class CreatePaymentTransactionCommandHandler : IConsumer<CreateTransactionCommand>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePaymentTransactionCommandHandler> _logger;

    public CreatePaymentTransactionCommandHandler(
        IPublishEndpoint publishEndpoint,
        IUnitOfWork unitOfWork,
        ILogger<CreatePaymentTransactionCommandHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateTransactionCommand> context)
    {
        var paymentTransaction = await _unitOfWork.PaymentTransactions.GetByBookingIdAsync(context.Message.BookingId);

        // Create new transaction
        if (paymentTransaction == null)
        {
            paymentTransaction = new PaymentTransaction()
            {
                Id = context.Message.BookingId,
                BookingId = context.Message.BookingId,
                PaymentAmount = context.Message.Amount,
                TransactionStatus = PaymentTransactionStatus.Pending,
                RefundStatus = RefundStatus.RefundNotNeeded,
                PaymentSessionStatus = PaymentSessionStatus.Valid,
                StripeSessionId = string.Empty,
                StripePaymentIntendId = string.Empty,
                StripePaymentSessionUrl = string.Empty
            };

            await _unitOfWork.PaymentTransactions.CreateAsync(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            await PublishExecutedAsync(context, paymentTransaction, isSuccessful: true);

            _logger.LogInformation("Started new payment transaction for booking {BookingId}",  paymentTransaction.BookingId);
        }

        // Already created payment transaction
        else if (paymentTransaction.TransactionStatus == PaymentTransactionStatus.Pending)
        {
            await PublishExecutedAsync(context, paymentTransaction, isSuccessful: true);

            _logger.LogInformation("Tried to start already existing payment transaction for booking with id {BookingId}",  paymentTransaction.BookingId);
        }

        // Tried to start already failed or expired transaction
        else
        {
            await PublishExecutedAsync(context, paymentTransaction, isSuccessful: false);

            _logger.LogInformation("Tried to start payment transaction that is {Status} for booking with id {BookingId}", paymentTransaction.TransactionStatus.ToString(), paymentTransaction.BookingId);
        }
    }

    private Task PublishExecutedAsync(ConsumeContext<CreateTransactionCommand> context, PaymentTransaction paymentTransaction, bool isSuccessful)
    {
        return _publishEndpoint.Publish(new TransactionCreateCommandExecuted()
        {
            BookingId = paymentTransaction.BookingId,
            PaymentTransactionId = paymentTransaction.Id,
            PaymentAmount = paymentTransaction.PaymentAmount,
            ApartmentName = context.Message.ApartmentName,
            CheckIn = context.Message.CheckIn,
            CheckOut = context.Message.CheckOut,
            GuestCount = context.Message.GuestCount,
            IsSuccessful = isSuccessful
        });
    }
}

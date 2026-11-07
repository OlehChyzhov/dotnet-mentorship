using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Success;

public class FinishPaymentTransactionCommandHandler : IConsumer<UpdateTransactionAsSuccessCommand>
{
    private readonly ILogger<FinishPaymentTransactionCommandHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ApplicationDbContext  _context;
    
    public FinishPaymentTransactionCommandHandler(
        ILogger<FinishPaymentTransactionCommandHandler> logger,
        IPublishEndpoint publishEndpoint,
        ApplicationDbContext context)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _context = context;
    }
    public async Task Consume(ConsumeContext<UpdateTransactionAsSuccessCommand> context)
    {
        try
        {
            var transaction =
                _context.PaymentTransactions.First(transaction =>
                    transaction.Id == context.Message.PaymentTransactionId);
            if (context.Message.PaymentStatus == PaymentStatus.Paid)
            {
                transaction.TransactionStatus = PaymentTransactionStatus.Succeeded;
            }
            else
            {
                transaction.TransactionStatus = PaymentTransactionStatus.Expired;
            }

            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new UpdateTransactionAsSuccessCommandExecuted()
            {
                TransactionId = context.Message.PaymentTransactionId,
                BookingId = context.Message.BookingId,
                IsSuccessful = true
            });
        }
        catch (Exception ex)
        {
            await _publishEndpoint.Publish(new UpdateTransactionAsSuccessCommandExecuted()
            {
                TransactionId = context.Message.PaymentTransactionId,
                BookingId = context.Message.BookingId,
                IsSuccessful = false
            });

            throw;
        }

    }
}
using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Airbnb.Payment.Infrastructure.Saga.BookingPayment.Success;

public class CreatePaymentTransactionCommandHandler : IConsumer<CreateTransactionCommand>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CreatePaymentTransactionCommandHandler> _logger;
    
    public CreatePaymentTransactionCommandHandler(
        IPublishEndpoint publishEndpoint, 
        ApplicationDbContext dbContext, 
        ILogger<CreatePaymentTransactionCommandHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<CreateTransactionCommand> context)
    {
        PaymentTransaction? paymentTransaction = _dbContext.PaymentTransactions.FirstOrDefault(x => x.BookingId == context.Message.BookingId);

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
            
            _dbContext.PaymentTransactions.Add(paymentTransaction);
            await _dbContext.SaveChangesAsync(context.CancellationToken);
            
            await _publishEndpoint.Publish(new TransactionCreateCommandExecuted()
            {
                BookingId = paymentTransaction.BookingId,
                PaymentTransactionId = paymentTransaction.Id,
                PaymentAmount = paymentTransaction.PaymentAmount,
                ApartmentName = context.Message.ApartmentName,
                CheckIn = context.Message.CheckIn,
                CheckOut = context.Message.CheckOut,
                GuestCount = context.Message.GuestCount,
                IsSuccessful = true
            });
            
            _logger.LogInformation("Started new payment transaction for booking {BookingId}",  paymentTransaction.BookingId);
        }
        
        // Already created payment transaction
        else if (paymentTransaction.TransactionStatus == PaymentTransactionStatus.Pending)
        {
            await _publishEndpoint.Publish(new TransactionCreateCommandExecuted()
            {
                BookingId = paymentTransaction.BookingId,
                PaymentTransactionId = paymentTransaction.Id,
                PaymentAmount = paymentTransaction.PaymentAmount,
                ApartmentName = context.Message.ApartmentName,
                CheckIn = context.Message.CheckIn,
                CheckOut = context.Message.CheckOut,
                GuestCount = context.Message.GuestCount,
                IsSuccessful = true
            });
            
            _logger.LogInformation("Tried to start already existing payment transaction for booking with id {BookingId}",  paymentTransaction.BookingId);
        }
        
        // Tried to start already failed or expired transaction
        else
        {
            await _publishEndpoint.Publish(new TransactionCreateCommandExecuted()
            {
                BookingId = paymentTransaction.BookingId,
                PaymentTransactionId = paymentTransaction.Id,
                PaymentAmount = paymentTransaction.PaymentAmount,
                ApartmentName = context.Message.ApartmentName,
                CheckIn = context.Message.CheckIn,
                CheckOut = context.Message.CheckOut,
                GuestCount = context.Message.GuestCount,
                IsSuccessful = false
            });
            
            _logger.LogInformation("Tried to start payment transaction that is {Status} for booking with id {BookingId}", paymentTransaction.TransactionStatus.ToString(), paymentTransaction.BookingId);
        }
    }
}
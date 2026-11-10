using Airbnb.Application.Abstractions.Repositories;
using Airbnb.Domain.Enums;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Airbnb.Infrastructure.Saga.BookingSaga.Success;

public class FinishBookingCommandHandler : IConsumer<UpdateBookingAsSuccessCommand>
{
    private readonly ILogger<FinishBookingCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public FinishBookingCommandHandler(
        ILogger<FinishBookingCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<UpdateBookingAsSuccessCommand> context)
    {
        var booking = await _unitOfWork.Bookings.GetByIdTrackedAsync(context.Message.BookingId);

        if (context.Message.Status == PaymentStatus.Paid)
        {
            booking.Status = BookingStatus.PaymentSucceeded;
        }
        else
        {
            booking.Status = BookingStatus.PaymentExpired;
        }

        await _unitOfWork.SaveChangesAsync();
        await _publishEndpoint.Publish(new UpdateBookingAsSuccessCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
            PaymentStatus = context.Message.Status
        });
    }
}
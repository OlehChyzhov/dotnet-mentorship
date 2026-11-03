using Airbnb.Application.Abstractions.Repositories;
using Airbnb.Domain.Enums;
using Airbnb.Shared.Contracts.Events.Payment;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Commands;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Events;
using MassTransit;

namespace Airbnb.Infrastructure.Saga.BookingSaga;

public class UpdateBookingRecordCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint) : IConsumer<UpdateBookingRecordCommand>
{
    public async Task Consume(ConsumeContext<UpdateBookingRecordCommand> context)
    {
        var booking = await unitOfWork.Bookings.GetByIdAsync(context.Message.BookingId);
        
        if (context.Message.Success)
        {
            booking.Status = BookingStatus.Paid;
        }
        else
        {
            booking.Status = BookingStatus.PaymentFailed;
        }
        
        await unitOfWork.SaveChangesAsync();
    }
}
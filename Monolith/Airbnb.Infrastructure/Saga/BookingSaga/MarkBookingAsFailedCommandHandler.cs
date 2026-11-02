using Airbnb.Application.Abstractions.Repositories;
using Airbnb.Domain.Enums;
using Airbnb.Shared.Contracts.Commands.Booking;
using MassTransit;

namespace Airbnb.Infrastructure.Saga.BookingSaga;

public class MarkBookingAsFailedCommandHandler(IUnitOfWork unitOfWork) : IConsumer<MarkBookingAsFailedCommand>
{
    public async Task Consume(ConsumeContext<MarkBookingAsFailedCommand> context)
    {
        var booking = await unitOfWork.Bookings.GetByIdAsync(context.Message.BookingId);
        booking.Status = BookingStatus.PaymentFailed;
        await unitOfWork.SaveChangesAsync();
    }
}
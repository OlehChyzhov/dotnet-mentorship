using Airbnb.Application.Abstractions.Repositories;
using Airbnb.Domain.Enums;
using Airbnb.Shared.Contracts.Commands.Booking;
using MassTransit;

namespace Airbnb.Infrastructure.Saga.BookingSaga;

public class MarkBookingAsPaidCommandHandler(IUnitOfWork unitOfWork) : IConsumer<MarkBookingAsPaidCommand>
{
    public async Task Consume(ConsumeContext<MarkBookingAsPaidCommand> context)
    {
        var booking = await unitOfWork.Bookings.GetByIdAsync(context.Message.BookingId);
        booking.Status = BookingStatus.Paid;
        await unitOfWork.SaveChangesAsync();
    }
}
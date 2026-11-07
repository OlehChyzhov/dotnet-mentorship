using Airbnb.Application.Abstractions.Repositories;
using Airbnb.Domain.Enums;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
using MassTransit;

namespace Airbnb.Infrastructure.Saga.BookingSaga.Failure;

public class RevertUpdatingBookingAsSuccessCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint) : IConsumer<RevertUpdatingBookingAsSuccessCommand>
{
    public async Task Consume(ConsumeContext<RevertUpdatingBookingAsSuccessCommand> context)
    {
        var booking = await unitOfWork.Bookings.GetByIdTrackedAsync(context.Message.BookingId);
        booking.Status = BookingStatus.PaymentFailed;
        await unitOfWork.SaveChangesAsync();

        await publishEndpoint.Publish(new RevertUpdatingBookingAsSuccessCommandExecuted()
        {
            TransactionId = context.Message.TransactionId,
            BookingId = context.Message.BookingId,
        });
    }
}
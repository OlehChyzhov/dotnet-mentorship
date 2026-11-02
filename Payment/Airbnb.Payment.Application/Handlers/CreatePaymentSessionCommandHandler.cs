using Airbnb.Shared.Contracts.Commands.Payment;
using Airbnb.Shared.Contracts.Events.Payment;
using MassTransit;

namespace Airbnb.Payment.Application.Handlers;

public class CreatePaymentSessionCommandHandler : IConsumer<CreatePaymentSessionCommand>
{
    public async Task Consume(ConsumeContext<CreatePaymentSessionCommand> context)
    {
        try
        {
            var random = new Random();
            var value = random.Next(0, 10);

            if (value <= 5)
            {
                await context.RespondAsync(new PaymentSessionCreationSucceededEvent()
                {
                    PaymentSessionUrl = $"http://payment.example.com?bookingid={context.Message.BookingId}&amount={context.Message.PaymentAmount}"
                });
            }
            else
            {
                await context.RespondAsync(new PaymentSessionCreationFailedEvent()
                {
                    FailureReason = "Because I said so"
                });
            }
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new PaymentSessionCreationFailedEvent()
            {
                FailureReason = ex.Message
            });
        }

    }
}
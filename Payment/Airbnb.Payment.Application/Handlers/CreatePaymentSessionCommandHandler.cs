using Airbnb.Payment.Application.Abstractions;
using Airbnb.Shared.Contracts.Events.Payment;
using Airbnb.Shared.Contracts.Messages;
using MassTransit;

namespace Airbnb.Payment.Application.Handlers;

public class CreatePaymentSessionCommandHandler(IPaymentService paymentService) : IConsumer<CreatePaymentSessionMessage>
{
    public async Task Consume(ConsumeContext<CreatePaymentSessionMessage> context)
    {
        var sessionDto = paymentService.CreatePaymentSession(context.Message);

        if (sessionDto.IsSuccessful)
        {
            await context.RespondAsync(new PaymentSessionCreationSucceededEvent()
            {
                PaymentSessionUrl = sessionDto.Value!.PaymentSessionUrl
            });
        }
        else
        {
            await context.RespondAsync(new PaymentSessionCreationFailedEvent()
            {
                FailureReason = sessionDto.Message!
            });
        }
    }
}
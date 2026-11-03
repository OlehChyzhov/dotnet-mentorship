using Airbnb.Shared.Contracts.Events.Booking;
using Airbnb.Shared.Contracts.Events.Payment;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Commands;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Events;
using MassTransit;

namespace Airbnb.Infrastructure.Saga.BookingSaga;

public class BookingPaymentStateMachineInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Guid? BookingId { get; set; }
}

public class BookingPaymentSagaStateMachine : MassTransitStateMachine<BookingPaymentStateMachineInstance>
{
    public State AwaitingPaymentTransactionToGetCreatedState { get; set; }
    public State AwaitingPaymentWebhookToGetTriggeredState { get; set; }
    public State UpdatingPaymentTransactionState { get; set; }
    public State UpdatingBookingRecordState { get; set; }
    
    public Event<BookingCreatedEvent> BookingCreatedEventExecuted { get; set; }
    public Event<PaymentTransactionCreatedEvent> PaymentTransactionCreatedEventExecuted { get; set; }
    public Event<PaymentWebhookTriggeredEvent>  PaymentWebhookTriggeredEventExecuted { get; set; }
    public Event<PaymentTransactionUpdatedEvent> PaymentTransactionUpdatedEventExecuted { get; set; }
    
    public BookingPaymentSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingCreatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentTransactionCreatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentWebhookTriggeredEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentTransactionUpdatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));

        Initially(
            When(BookingCreatedEventExecuted)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                })
                .TransitionTo(AwaitingPaymentTransactionToGetCreatedState)
                .Publish(context => new CreatePaymentTransactionCommand()
                {
                    BookingId = context.Message.BookingId,
                    Amount = context.Message.BookingTotal
                }));
        
        During(AwaitingPaymentTransactionToGetCreatedState,
            When(PaymentTransactionCreatedEventExecuted)
                .IfElse(ctx => ctx.Message.IsCreated,
                    created => created.TransitionTo(AwaitingPaymentWebhookToGetTriggeredState),
                    failed  => failed
                        .Publish(ctx => new UpdateBookingRecordCommand
                        {
                            BookingId = ctx.Message.BookingId,
                            IsFailed  = true,
                        })
                        .Finalize()));
        
        During(AwaitingPaymentWebhookToGetTriggeredState,
            When(PaymentWebhookTriggeredEventExecuted)
                .TransitionTo(UpdatingPaymentTransactionState)
                .Publish(context => new UpdatePaymentTransactionCommand()
                {
                    BookingId = context.Message.BookingId,
                    Success = context.Message.Success,
                    PaymentIntentId =  context.Message.PaymentIntentId,
                    PaymentSessionId =  context.Message.PaymentSessionId,
                    FailureReason =  context.Message.FailureReason,
                }));
        
        During(UpdatingPaymentTransactionState,
            When(PaymentTransactionUpdatedEventExecuted)
                .TransitionTo(UpdatingBookingRecordState)
                .Publish(context => new UpdateBookingRecordCommand()
                {
                    BookingId = context.Message.BookingId,
                    IsFailed = context.Message.IsFailed,
                    IsExpired = context.Message.IsExpired,
                    IsSuccessful = context.Message.IsSuccessful,
                })
                .Finalize());
        
        SetCompletedWhenFinalized();
    }
}
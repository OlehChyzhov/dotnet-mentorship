using Airbnb.Shared.Contracts.Commands.Booking;
using Airbnb.Shared.Contracts.Events.Booking;
using Airbnb.Shared.Contracts.Events.Payment;
using MassTransit;

namespace Airbnb.Infrastructure.Saga.BookingSaga;

public class BookingPaymentStateMachineInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Guid? BookingId { get; set; }
    public string? PaymentIntendId { get; set; }
    public string? FailureReason { get; set; }
}

public class BookingPaymentSagaStateMachine : MassTransitStateMachine<BookingPaymentStateMachineInstance>
{
    public State AwaitingPayment { get; set; }
    public State PaymentSuccededState { get; set; }
    public State PaymentFailedState { get; set; }
    
    public Event<BookingCreatedEvent> BookingCreatedEvent { get; set; }
    public Event<PaymentSucceededEvent> PaymentSucceededEvent { get; set; }
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }
    
    public BookingPaymentSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingCreatedEvent, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentSucceededEvent, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentFailedEvent, e => e.CorrelateById(m => m.Message.BookingId));

        Initially(
            When(BookingCreatedEvent)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                })
                .TransitionTo(AwaitingPayment));

        During(AwaitingPayment,
            When(PaymentSucceededEvent)
                .Then(context =>
                {
                    context.Saga.PaymentIntendId = context.Message.PaymentIntentId;
                })
                .TransitionTo(PaymentSuccededState)
                .Publish(context => new MarkBookingAsPaidCommand() { BookingId = context.Message.BookingId })
                .Finalize());
        
        
        During(AwaitingPayment,
            When(PaymentFailedEvent)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .TransitionTo(PaymentFailedState)
                .Publish(context => new MarkBookingAsFailedCommand() {BookingId = context.Message.BookingId })
                .Finalize());
    }
}
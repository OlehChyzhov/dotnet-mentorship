using Airbnb.Shared.Contracts.Events.Booking;
using Airbnb.Shared.Contracts.Events.Payment;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Success;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Commands;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;
using MassTransit;

namespace Airbnb.Infrastructure.Saga.BookingSaga;

public class BookingPaymentStateMachineInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
}

public class BookingPaymentSagaStateMachine : MassTransitStateMachine<BookingPaymentStateMachineInstance>
{
    // Start of the Saga
    public Event<BookingCreatedEvent> BookingCreatedEventExecuted { get; set; }
    
    // Initial State
    public State AwaitingPaymentTransactionToGetCreated { get; set; }
    public Event<CreatePaymentTransactionCommandExecuted> PaymentTransactionCreatedEventExecuted { get; set; }
    
    
    // Happy Path
    public State AwaitingPaymentSessionToGetCreated { get; set; }
    public State AwaitingPaymentWebhookToGetTriggered { get; set; }
    public State FinishingBooking { get; set; }
    public State FinishingPaymentTransaction { get; set; }
    
    public Event<CreatePaymentSessionCommandExecuted> PaymentSessionCreatedEventExecuted { get; set; }
    public Event<PaymentWebhookTriggeredEvent>  PaymentWebhookTriggeredEventExecuted { get; set; }
    public Event<FinishBookingCommandExecuted> BookingFinishedEventExecuted { get; set; }
    public Event<FinishPaymentTransactionCommandExecuted> PaymentTransactionFinishedCommandExecuted { get; set; }
    
    
    // Fail Path
    public State StartRevertingPaymentTransaction { get; set; }
    public State InvalidatingPaymentSession { get; set; }
    public State RevertingPaymentIfWasPaid { get; set; }
    public State RevertingBooking { get; set; }
    public State RevertingPaymentTransaction { get; set; }
    
    
    public BookingPaymentSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingCreatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        
        // Happy Path
        Event(() => PaymentTransactionCreatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentSessionCreatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentWebhookTriggeredEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingFinishedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentTransactionFinishedCommandExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        
        // Fail Path
        
        
        // Initial State
        Initially(
            When(BookingCreatedEventExecuted)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                })
                .TransitionTo(AwaitingPaymentTransactionToGetCreated)
                .Publish(context => new CreatePaymentTransactionCommand()
                {
                    BookingId = context.Message.BookingId,
                    Amount = context.Message.TotalAmount
                }));
        
        // Happy Path
        During(AwaitingPaymentTransactionToGetCreated,
            When(PaymentTransactionCreatedEventExecuted)
                .IfElse(ctx => ctx.Message.IsSuccessful,
                    created => created
                        .Then(context =>
                        {
                            context.Saga.PaymentTransactionId = context.Message.PaymentTransactionId;
                        })
                        .TransitionTo(AwaitingPaymentSessionToGetCreated)
                        .Publish(ctx => new CreatePaymentSessionCommand()
                        {
                            BookingId = ctx.Message.BookingId,
                            PaymentAmount = ctx.Message.PaymentAmount,
                            PaymentTransactionId = ctx.Message.PaymentTransactionId,
                            ApartmentName = ctx.Message.ApartmentName,
                            CheckIn = ctx.Message.CheckIn,
                            CheckOut = ctx.Message.CheckOut,
                            GuestCount = ctx.Message.GuestCount,
                        }),
                    
                    failed  => failed
                        .TransitionTo(StartRevertingPaymentTransaction)
                        .Publish(context => new // TODO: START REVERTING
                        {

                        })));
        
        During(AwaitingPaymentSessionToGetCreated,
            When(PaymentSessionCreatedEventExecuted)
                .IfElse(ctx => ctx.Message.IsSuccessful,
                    created => created
                        .TransitionTo(AwaitingPaymentWebhookToGetTriggered),

                    failed => failed
                        .TransitionTo(StartRevertingPaymentTransaction)
                        .Publish(context => new // TODO: START REVERTING
                        {
                            
                        })));
        
        During(AwaitingPaymentWebhookToGetTriggered,
            When(PaymentWebhookTriggeredEventExecuted)
                .IfElse(ctx => ctx.Message.IsSuccessful,
                    success => success
                        .TransitionTo(FinishingBooking)
                        .Publish(context => new FinishBookingCommand()
                        {
                            TransactionId = context.Saga.PaymentTransactionId,
                            BookingId = context.Message.BookingId,
                            Status = context.Message.Status
                        }),
                    
                    failed => failed
                        .TransitionTo(StartRevertingPaymentTransaction)
                        .Publish(context => new // TODO: START REVERTING
                        {
                    
                        })));

        During(FinishingBooking,
            When(BookingFinishedEventExecuted)
                .IfElse(ctx => ctx.Message.IsSuccessful,
                    success => success
                        .TransitionTo(FinishingPaymentTransaction)
                        .Publish(context => new FinishPaymentTransactionCommand()
                        {
                            PaymentTransactionId = context.Message.TransactionId,
                            BookingId = context.Message.BookingId,
                            PaymentStatus = context.Message.PaymentStatus
                        }),
                    
                    failed => failed
                        .TransitionTo(StartRevertingPaymentTransaction)
                        .Publish(context => new // TODO: START REVERTING
                        {
                            
                        })));

        During(FinishingPaymentTransaction,
            When(PaymentTransactionFinishedCommandExecuted)
                .If(ctx => !ctx.Message.IsSuccessful,
                    failed => failed
                        .TransitionTo(StartRevertingPaymentTransaction)
                        .Publish(context => new // TODO: Start Reverting
                        {
                            
                        })));
        
        // Fail Path

        
        
        SetCompletedWhenFinalized();
    }
}
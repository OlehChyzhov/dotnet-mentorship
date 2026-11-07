using Airbnb.Shared.Contracts.Events.Booking;
using Airbnb.Shared.Contracts.Saga.BookingPayment;
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
    public State AwaitingTransactionCreate { get; set; }
    public Event<TransactionCreateCommandExecuted> TransactionCreateCommandExecuted { get; set; }
    
    
    // Happy Path
    public State AwaitingPaymentSessionCreate { get; set; }
    public State AwaitingStripeWebhook { get; set; }
    public State UpdatingBookingAsSuccess { get; set; }
    public State UpdatingTransactionAsSuccess { get; set; }
    
    public Event<CreatePaymentSessionCommandExecuted> PaymentSessionCreateEventExecuted { get; set; }
    public Event<PaymentWebhookTriggeredEvent>  PaymentWebhookTriggeredEventExecuted { get; set; }
    public Event<UpdateBookingAsSuccessCommandExecuted> BookingFinishedEventExecuted { get; set; }
    public Event<UpdateTransactionAsSuccessCommandExecuted> PaymentTransactionFinishedCommandExecuted { get; set; }
    
    public Event<Fault<CreateTransactionCommand>> CreateTransactionCommandFaulted { get; set; }
    public Event<Fault<CreatePaymentSessionCommand>> CreatePaymentSessionCommandFaulted { get; set; }
    public Event<Fault<UpdateBookingAsSuccessCommand>> UpdateBookingAsSuccessCommandFaulted { get; set; }
    public Event<Fault<UpdateTransactionAsSuccessCommand>> UpdateTransactionAsSuccessCommandFaulted { get; set; }
    
    
    // Fail Path
    public State RevertingUpdatingTransactionAsSuccess { get; set; }
    public State RevertingUpdatingBookingAsSuccess { get; set; }
    public State RefundingPaidAmount { get; set; }
    public State InvalidatingPaymentSession { get; set; }
    public State RevertingTransactionCreate { get; set; }
    
    public Event<RevertUpdatingTransactionAsSuccessCommandExecuted> RevertingUpdatingTransactionAsSuccessCommandExecuted { get; set; }
    public Event<RevertUpdatingBookingAsSuccessCommandExecuted> RevertingUpdatingBookingAsSuccessCommandExecuted { get; set; }
    public Event<RefundingPaidAmountCommandExecuted> RefundingPaidAmountCommandExecuted { get; set; }
    public Event<InvalidatePaymentSessionCommandExecuted> InvalidatingPaymentSessionCommandExecuted { get; set; }
    public Event<RevertingTransactionCreateCommandExecuted> RevertingTransactionCreateExecuted { get; set; }
    
    public BookingPaymentSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingCreatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        
        // Happy Path
        Event(() => TransactionCreateCommandExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentSessionCreateEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentWebhookTriggeredEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingFinishedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentTransactionFinishedCommandExecuted, e => e.CorrelateById(m => m.Message.BookingId));

        Event(() => CreateTransactionCommandFaulted, e => e.CorrelateById(m => m.Message.Message.BookingId));
        Event(() => CreatePaymentSessionCommandFaulted, e => e.CorrelateById(m => m.Message.Message.BookingId));
        Event(() => UpdateBookingAsSuccessCommandFaulted, e => e.CorrelateById(m => m.Message.Message.BookingId));
        Event(() => UpdateTransactionAsSuccessCommandFaulted, e => e.CorrelateById(m => m.Message.Message.BookingId));
        
        // Fail Path
        Event(() => RevertingUpdatingTransactionAsSuccessCommandExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => RevertingUpdatingBookingAsSuccessCommandExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => RefundingPaidAmountCommandExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => InvalidatingPaymentSessionCommandExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        Event(() => RevertingTransactionCreateExecuted, e => e.CorrelateById(m => m.Message.BookingId));
        
        // Initial State
        Initially(
            When(BookingCreatedEventExecuted)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                })
                .TransitionTo(AwaitingTransactionCreate)
                .Publish(context => new CreateTransactionCommand()
                {
                    BookingId = context.Message.BookingId,
                    Amount = context.Message.TotalAmount,
                    ApartmentName = context.Message.ApartmentName,
                    CheckIn = context.Message.CheckIn,
                    CheckOut = context.Message.CheckOut,
                    GuestCount = context.Message.GuestCount
                }));
        
        // Happy Path
        During(AwaitingTransactionCreate,
            When(TransactionCreateCommandExecuted)
                .Then(context =>
                {
                    context.Saga.PaymentTransactionId = context.Message.PaymentTransactionId;
                    context.Saga.BookingId = context.Message.BookingId;
                })
                .TransitionTo(AwaitingPaymentSessionCreate)
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
            
            When(CreateTransactionCommandFaulted)
                .TransitionTo(RevertingTransactionCreate)
                .Publish(context => new RevertTransactionCreateCommand()
                    {
                        TransactionId = context.Saga.PaymentTransactionId,
                        BookingId = context.Saga.BookingId
                    }));
        
        During(AwaitingPaymentSessionCreate,
            When(PaymentSessionCreateEventExecuted)
                .TransitionTo(AwaitingStripeWebhook),
            
            When(CreatePaymentSessionCommandFaulted)
                .TransitionTo(InvalidatingPaymentSession)
                .Publish(context => new InvalidatePaymentSessionCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId,
                }));
        
        During(AwaitingStripeWebhook,
            When(PaymentWebhookTriggeredEventExecuted)
                .IfElse(ctx => ctx.Message.IsSuccessful,
                    success => success
                        .TransitionTo(UpdatingBookingAsSuccess)
                        .Publish(context => new UpdateBookingAsSuccessCommand()
                        {
                            TransactionId = context.Saga.PaymentTransactionId,
                            BookingId = context.Message.BookingId,
                            Status = context.Message.Status
                        }),
                    
                    failed => failed
                        .TransitionTo(RefundingPaidAmount)
                        .Publish(context => new RefundPaidAmountCommand()
                        {
                            TransactionId = context.Saga.PaymentTransactionId,
                            BookingId = context.Message.BookingId,
                        })));

        During(UpdatingBookingAsSuccess,
            When(BookingFinishedEventExecuted)
                .TransitionTo(UpdatingTransactionAsSuccess)
                .Publish(context => new UpdateTransactionAsSuccessCommand()
                {
                    PaymentTransactionId = context.Message.TransactionId,
                    BookingId = context.Message.BookingId,
                    PaymentStatus = context.Message.PaymentStatus
                }),
            
            When(UpdateBookingAsSuccessCommandFaulted)
                .TransitionTo(RevertingUpdatingBookingAsSuccess)
                .Publish(context => new RevertUpdatingBookingAsSuccessCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId,
                }));
                
        During(UpdatingTransactionAsSuccess,
            When(PaymentTransactionFinishedCommandExecuted)
                .Finalize(),
            
            When(UpdateTransactionAsSuccessCommandFaulted)
                .TransitionTo(RevertingUpdatingTransactionAsSuccess)
                .Publish(context => new RevertUpdatingTransactionAsSuccessCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId
                }));
        
        
        // Fail Path
        During(RevertingUpdatingTransactionAsSuccess,
            When(RevertingUpdatingTransactionAsSuccessCommandExecuted)
                .TransitionTo(RevertingUpdatingBookingAsSuccess)
                .Publish(context => new RevertUpdatingBookingAsSuccessCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId
                }));
        
        During(RevertingUpdatingBookingAsSuccess,
            When(RevertingUpdatingBookingAsSuccessCommandExecuted)
                .TransitionTo(RefundingPaidAmount)
                .Publish(context => new RefundPaidAmountCommand()
                {
                   TransactionId = context.Saga.PaymentTransactionId,
                   BookingId = context.Saga.BookingId
                }));
        
        During(RefundingPaidAmount,
            When(RefundingPaidAmountCommandExecuted)
                .TransitionTo(InvalidatingPaymentSession)
                .Publish(context => new InvalidatePaymentSessionCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId,
                }));

        During(InvalidatingPaymentSession,
            When(InvalidatingPaymentSessionCommandExecuted)
                .TransitionTo(RevertingTransactionCreate)
                .Publish(context => new RevertTransactionCreateCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId,
                }));
        
        During(RevertingTransactionCreate,
            When(RevertingTransactionCreateExecuted)
                .Finalize());
        
        SetCompletedWhenFinalized();
    }
}
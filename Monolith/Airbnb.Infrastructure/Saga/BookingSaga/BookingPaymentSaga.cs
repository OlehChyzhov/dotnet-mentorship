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
    public DateTime CreatedAt { get; set; }
    public string? FailureReason { get; set; }
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

    public State CompensationFailed { get; set; }

    public Event<RevertUpdatingTransactionAsSuccessCommandExecuted> RevertingUpdatingTransactionAsSuccessCommandExecuted { get; set; }
    public Event<RevertUpdatingBookingAsSuccessCommandExecuted> RevertingUpdatingBookingAsSuccessCommandExecuted { get; set; }
    public Event<RefundingPaidAmountCommandExecuted> RefundingPaidAmountCommandExecuted { get; set; }
    public Event<InvalidatePaymentSessionCommandExecuted> InvalidatingPaymentSessionCommandExecuted { get; set; }
    public Event<RevertingTransactionCreateCommandExecuted> RevertingTransactionCreateExecuted { get; set; }

    public Event<Fault<RevertUpdatingTransactionAsSuccessCommand>> RevertUpdatingTransactionAsSuccessCommandFaulted { get; set; }
    public Event<Fault<RevertUpdatingBookingAsSuccessCommand>> RevertUpdatingBookingAsSuccessCommandFaulted { get; set; }
    public Event<Fault<RefundPaidAmountCommand>> RefundPaidAmountCommandFaulted { get; set; }
    public Event<Fault<InvalidatePaymentSessionCommand>> InvalidatePaymentSessionCommandFaulted { get; set; }
    public Event<Fault<RevertTransactionCreateCommand>> RevertTransactionCreateCommandFaulted { get; set; }

    public BookingPaymentSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingCreatedEventExecuted, e => e.CorrelateById(m => m.Message.BookingId));

        // Happy Path
        Event(() => TransactionCreateCommandExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => PaymentSessionCreateEventExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => PaymentWebhookTriggeredEventExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => BookingFinishedEventExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => PaymentTransactionFinishedCommandExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });

        Event(() => CreateTransactionCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => CreatePaymentSessionCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => UpdateBookingAsSuccessCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => UpdateTransactionAsSuccessCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });

        // Fail Path
        Event(() => RevertingUpdatingTransactionAsSuccessCommandExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => RevertingUpdatingBookingAsSuccessCommandExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => RefundingPaidAmountCommandExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => InvalidatingPaymentSessionCommandExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => RevertingTransactionCreateExecuted, e => { e.CorrelateById(m => m.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });

        Event(() => RevertUpdatingTransactionAsSuccessCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => RevertUpdatingBookingAsSuccessCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => RefundPaidAmountCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => InvalidatePaymentSessionCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });
        Event(() => RevertTransactionCreateCommandFaulted, e => { e.CorrelateById(m => m.Message.Message.BookingId); e.OnMissingInstance(m => m.Discard()); });

        // Initial State
        Initially(
            When(BookingCreatedEventExecuted)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                    context.Saga.PaymentTransactionId = context.Message.BookingId;
                    context.Saga.CreatedAt = DateTime.UtcNow;
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
                .IfElse(context => context.Message.IsSuccessful,
                    success => success
                        .Then(context =>
                        {
                            context.Saga.PaymentTransactionId = context.Message.PaymentTransactionId;
                        })
                        .TransitionTo(AwaitingPaymentSessionCreate)
                        .Publish(context => new CreatePaymentSessionCommand()
                        {
                            BookingId = context.Message.BookingId,
                            PaymentAmount = context.Message.PaymentAmount,
                            PaymentTransactionId = context.Message.PaymentTransactionId,
                            ApartmentName = context.Message.ApartmentName,
                            CheckIn = context.Message.CheckIn,
                            CheckOut = context.Message.CheckOut,
                            GuestCount = context.Message.GuestCount,
                        }),

                    failed => failed
                        .Then(context =>
                        {
                            context.Saga.FailureReason = "Payment transaction for the booking is already closed";
                        })
                        .TransitionTo(RevertingUpdatingBookingAsSuccess)
                        .Publish(context => new RevertUpdatingBookingAsSuccessCommand()
                        {
                            TransactionId = context.Saga.PaymentTransactionId,
                            BookingId = context.Saga.BookingId
                        })),

            When(CreateTransactionCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(CreateTransactionCommand)} failed";
                })
                .TransitionTo(RevertingUpdatingBookingAsSuccess)
                .Publish(context => new RevertUpdatingBookingAsSuccessCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId
                }));

        During(AwaitingPaymentSessionCreate,
            When(PaymentSessionCreateEventExecuted)
                .TransitionTo(AwaitingStripeWebhook),
            
            When(PaymentWebhookTriggeredEventExecuted)
                .TransitionTo(UpdatingBookingAsSuccess)
                .Publish(context => new UpdateBookingAsSuccessCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Message.BookingId,
                    Status = context.Message.Status
                }),
            
            When(CreatePaymentSessionCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(CreatePaymentSessionCommand)} failed";
                })
                .TransitionTo(RevertingUpdatingBookingAsSuccess)
                .Publish(context => new RevertUpdatingBookingAsSuccessCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId
                }));
        
        During(AwaitingStripeWebhook,
            When(PaymentWebhookTriggeredEventExecuted)
                .TransitionTo(UpdatingBookingAsSuccess)
                .Publish(context => new UpdateBookingAsSuccessCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Message.BookingId,
                    Status = context.Message.Status
                }));

        During(UpdatingBookingAsSuccess,
            When(BookingFinishedEventExecuted)
                .TransitionTo(UpdatingTransactionAsSuccess)
                .Publish(context => new UpdateTransactionAsSuccessCommand()
                {
                    PaymentTransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Message.BookingId,
                    PaymentStatus = context.Message.PaymentStatus
                }),
            
            When(UpdateBookingAsSuccessCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(UpdateBookingAsSuccessCommand)} failed";
                })
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
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(UpdateTransactionAsSuccessCommand)} failed";
                })
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
                }),
            
            When(RevertUpdatingTransactionAsSuccessCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(RevertUpdatingTransactionAsSuccessCommand)} failed";
                })
                .TransitionTo(CompensationFailed));
        
        During(RevertingUpdatingBookingAsSuccess,
            When(RevertingUpdatingBookingAsSuccessCommandExecuted)
                .TransitionTo(RefundingPaidAmount)
                .Publish(context => new RefundPaidAmountCommand()
                {
                   TransactionId = context.Saga.PaymentTransactionId,
                   BookingId = context.Saga.BookingId
                }),
            
            When(RevertUpdatingBookingAsSuccessCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(RevertUpdatingBookingAsSuccessCommand)} failed";
                })
                .TransitionTo(CompensationFailed));
        
        During(RefundingPaidAmount,
            When(RefundingPaidAmountCommandExecuted)
                .TransitionTo(InvalidatingPaymentSession)
                .Publish(context => new InvalidatePaymentSessionCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId,
                }),
            
            When(RefundPaidAmountCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(RefundPaidAmountCommand)} failed";
                })
                .TransitionTo(CompensationFailed));

        During(InvalidatingPaymentSession,
            When(InvalidatingPaymentSessionCommandExecuted)
                .TransitionTo(RevertingTransactionCreate)
                .Publish(context => new RevertTransactionCreateCommand()
                {
                    TransactionId = context.Saga.PaymentTransactionId,
                    BookingId = context.Saga.BookingId,
                }),
            
            When(InvalidatePaymentSessionCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(InvalidatePaymentSessionCommand)} failed";
                })
                .TransitionTo(CompensationFailed));
        
        During(RevertingTransactionCreate,
            When(RevertingTransactionCreateExecuted)
                .Finalize(),
            
            When(RevertTransactionCreateCommandFaulted)
                .Then(context =>
                {
                    context.Saga.FailureReason = $"{nameof(RevertTransactionCreateCommand)} failed";
                })
                .TransitionTo(CompensationFailed));
        
        SetCompletedWhenFinalized();
    }
}

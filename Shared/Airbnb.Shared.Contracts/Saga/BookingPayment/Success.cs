using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;

namespace Airbnb.Shared.Contracts.Saga.BookingPayment;

// Step 1
public class CreateTransactionCommand
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    
    public string ApartmentName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
}

public class TransactionCreateCommandExecuted
{
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public  decimal PaymentAmount { get; set; }
    public string ApartmentName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
    public bool IsSuccessful { get; set; }
}

// Step 2
public class CreatePaymentSessionCommand
{
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public decimal PaymentAmount { get; set; }

    public string ApartmentName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
}

public class CreatePaymentSessionCommandExecuted
{
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? PaymentSessionId { get; set; }
    public string? PaymentSessionUrl { get; set; }
}

// Step 3
public class PaymentWebhookTriggeredEvent
{
    public Guid BookingId { get; set; }
    public string PaymentIntentId { get; set; }
    public string PaymentSessionId { get; set; }
    public PaymentStatus Status { get; set; }
}

// Step 4
public class UpdateBookingAsSuccessCommand
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
    public PaymentStatus Status { get; set; }
}

public class UpdateBookingAsSuccessCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

// Step 5
public class UpdateTransactionAsSuccessCommand
{ 
    public Guid PaymentTransactionId { get; set; }
    public Guid BookingId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

public class UpdateTransactionAsSuccessCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
}
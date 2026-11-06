using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;

namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;

public class FinishBookingCommandExecuted
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public bool IsSuccessful { get; set; }
}
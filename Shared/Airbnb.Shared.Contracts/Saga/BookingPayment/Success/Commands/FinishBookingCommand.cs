using Airbnb.Shared.Contracts.Saga.BookingPayment.Enums;
using Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Events;

namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Success.Commands;

public class FinishBookingCommand
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
    public PaymentStatus Status { get; set; }
}
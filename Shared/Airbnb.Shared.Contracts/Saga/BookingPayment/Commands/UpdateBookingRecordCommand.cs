namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Commands;

public class UpdateBookingRecordCommand
{
    public Guid BookingId { get; set; }
    public bool Success { get; set; }
}
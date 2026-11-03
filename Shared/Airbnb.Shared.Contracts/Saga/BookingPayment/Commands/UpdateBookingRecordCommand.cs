namespace Airbnb.Shared.Contracts.Saga.BookingPayment.Commands;

public class UpdateBookingRecordCommand
{
    public Guid BookingId { get; set; }
    public bool IsFailed { get; set; }
    public bool IsSuccessful { get; set; }
    public bool IsExpired { get; set; }
}
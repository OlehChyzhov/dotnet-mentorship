namespace Airbnb.Shared.Contracts.Commands.Booking;

public class MarkBookingAsPaidCommand
{
    public Guid BookingId { get; set; }
}
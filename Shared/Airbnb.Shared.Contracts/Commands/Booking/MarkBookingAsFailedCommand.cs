namespace Airbnb.Shared.Contracts.Commands.Booking;

public class MarkBookingAsFailedCommand
{
    public Guid BookingId { get; set; }
}
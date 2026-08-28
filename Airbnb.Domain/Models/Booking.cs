using Airbnb.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Domain.Models;

public class Booking : IEntity<Guid, Guid>
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    
    // Stay Dates
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    
    // Pricing at booking time (to avoid price changes affecting existing bookings)
    public double BookedPricePerNight { get; set; }
    public double BookedTotalPrice { get; set; }
    
    public int GuestsCount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relationships
    public Guid ApartmentId { get; set; }
    public Apartment? Apartment { get; set; }
    
    public string ClientId { get; set; } = string.Empty;
    public IdentityUser? Client { get; set; }
}
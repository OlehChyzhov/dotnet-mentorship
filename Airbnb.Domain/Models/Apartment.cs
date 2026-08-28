using Airbnb.Domain.Enums;

namespace Airbnb.Domain.Models;

public class Apartment : IEntity<Guid, Guid>
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    
    // Listing info
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ApartmentType Type { get; set; }
    
    // Location
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    // Capacity & Pricing
    public int MaxGuests { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Kitchens { get; set; }
    public int LivingRooms { get; set; }
    public double PricePerNight { get; set; }
    
    // Lifecycle
    public bool IsListed { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Ownership
    public string OwnerId { get; set; } = string.Empty;
    
    // Navigation
    public User? Owner { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    
}
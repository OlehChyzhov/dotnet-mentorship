using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Apartment;

public class CreateApartmentDto
{
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
    
    // Lifecycle & Ownership
    public bool IsListed { get; set; }
    public DateTime CreatedAt { get; set; }
}
using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Apartment;

public record CreateApartmentDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ApartmentType Type { get; init; }
    
    // Location
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    
    // Capacity & Pricing
    public int MaxGuests { get; init; }
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public int Kitchens { get; init; }
    public int LivingRooms { get; init; }
    public double PricePerNight { get; init; }
    
    // Lifecycle & Ownership
    public bool IsListed { get; init; }
}
namespace Airbnb.Shared.Contracts.Events.Apartment;

public record ApartmentCreatedEvent
{
    public string ApartmentId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}
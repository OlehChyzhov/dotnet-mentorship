namespace Airbnb.Shared.Contracts.Events.Apartment;

public record ApartmentDeletedEvent
{
    public string ApartmentId { get; set; } =  string.Empty;
    public DateTime DeletedAt { get; set; } =  DateTime.UtcNow;
}
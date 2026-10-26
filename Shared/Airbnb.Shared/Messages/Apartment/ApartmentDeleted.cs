namespace Airbnb.Shared.Messages.Apartment;

public record ApartmentDeleted
{
    public string EntityId { get; set; } =  string.Empty;
    public DateTime DeletedAt { get; set; } =  DateTime.UtcNow;
}
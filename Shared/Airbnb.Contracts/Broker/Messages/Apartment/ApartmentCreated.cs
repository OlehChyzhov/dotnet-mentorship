namespace Airbnb.Contracts.Broker.Messages.Apartment;

public record ApartmentCreated
{
    public string EntityId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}
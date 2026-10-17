namespace Airbnb.Contracts.Broker.Messages.Apartment;

public record ApartmentCreated
{
    public string EntityId { get; set; } = string.Empty;
}
namespace Airbnb.Contracts.Broker.Messages.Apartment;

public record ApartmentDeleted
{
    public string EntityId { get; set; } =  string.Empty;
}
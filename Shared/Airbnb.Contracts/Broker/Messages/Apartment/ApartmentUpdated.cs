namespace Airbnb.Contracts.Broker.Messages.Apartment;

public record ApartmentUpdated
{
    public string EntityId { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } =  DateTime.UtcNow;
}
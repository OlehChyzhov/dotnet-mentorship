namespace Airbnb.Domain.Models;

public interface IEntity<TKey, TExternalKey>
{
    public TKey Id { get; set; }
    
    public TExternalKey ExternalId { get; set; }
}
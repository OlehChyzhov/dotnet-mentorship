namespace Airbnb.Audit.Domain.Attributes;

public sealed class BsonCollection : Attribute
{
    public string CollectionName { get; }
    
    public BsonCollection(string collectionName)
    {
        CollectionName = collectionName;
    }
}
namespace Airbnb.Audit.Application.Options;

public class MongoDbOptions
{
    public string? DatabaseName { get; set; }
    public string? UserAuditsCollectionName { get; set; }
}
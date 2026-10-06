namespace Airbnb.Audit.Infrastructure.Options;

public class MongoDbOptions
{
    public string? DatabaseName { get; set; }
    public string? UserAuditsCollectionName { get; set; }
}
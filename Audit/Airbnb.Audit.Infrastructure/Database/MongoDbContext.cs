using Airbnb.Audit.Domain.Models.User;
using Airbnb.Audit.Infrastructure.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database;

public class MongoDbContext
{
    private readonly MongoDbOptions _options;
    private readonly IMongoClient _client;
    
    public MongoDbContext(IOptions<MongoDbOptions> options, IMongoClient mongoClient)
    {
        _options = options.Value;
        _client = mongoClient;
    }

    public IMongoCollection<UserBase> UserAuditsCollection
    {
        get
        {
            if (field == null)
            {
                var db = _client.GetDatabase(_options.DatabaseName);
                var collection = db.GetCollection<UserBase>(_options.UserAuditsCollectionName);
                field = collection;
            }
            
            return field;
        }
    }
}
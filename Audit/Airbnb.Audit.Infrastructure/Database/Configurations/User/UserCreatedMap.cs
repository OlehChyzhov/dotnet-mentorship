using Airbnb.Audit.Domain.Models.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Airbnb.Audit.Infrastructure.Database.Configurations.User;

public sealed class UserCreatedMap : BsonClassMap<UserCreated>
{
    public UserCreatedMap()
    {
        AutoMap();
        MapIdMember(x => x.Id)
            .SetIdGenerator(StringObjectIdGenerator.Instance)
            .SetSerializer(new StringSerializer(BsonType.ObjectId));
    }
}
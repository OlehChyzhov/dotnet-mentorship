using Airbnb.Audit.Domain.Models.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Airbnb.Audit.Infrastructure.Database.Configurations.User;

public class UserDeletedMap : BsonClassMap<UserDeleted>
{
    public UserDeletedMap()
    {
        AutoMap();
        MapIdMember(x => x.Id)
            .SetIdGenerator(StringObjectIdGenerator.Instance)
            .SetSerializer(new StringSerializer(BsonType.ObjectId));
    }
}
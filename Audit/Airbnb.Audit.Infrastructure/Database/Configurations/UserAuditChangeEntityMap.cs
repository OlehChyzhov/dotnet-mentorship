using Airbnb.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Airbnb.Audit.Infrastructure.Database.Configurations;

public class UserAuditChangeEntityMap : BsonClassMap<UserAuditChangeEntity>
{
    public UserAuditChangeEntityMap()
    {
        AutoMap();
        MapIdMember(x => x.Id)
            .SetIdGenerator(StringObjectIdGenerator.Instance)
            .SetSerializer(new StringSerializer(BsonType.ObjectId));
    }
}
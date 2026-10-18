using Airbnb.Audit.Domain.Enums;
using Airbnb.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Airbnb.Audit.Infrastructure.Database.CollectionConfigs;

public class ApartmentAuditEntityConfigs : BsonClassMap<ApartmentAuditEntity>
{
    public ApartmentAuditEntityConfigs()
    {
        AutoMap();
        
        MapIdMember(x => x.Id)
            .SetIdGenerator(StringObjectIdGenerator.Instance)
            .SetSerializer(new StringSerializer(BsonType.ObjectId));
        
        MapMember(x => x.ChangeType)
            .SetSerializer(new NullableSerializer<ChangeType>(new EnumSerializer<ChangeType>(BsonType.String)));
    }
}
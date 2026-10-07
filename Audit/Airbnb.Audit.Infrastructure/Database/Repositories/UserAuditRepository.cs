using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Models;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class UserAuditRepository : MongoRepository<UserAuditChangeEntity>, IUserAuditRepository
{
    public UserAuditRepository(IMongoDatabase db, FilterDefinitionBuilder<UserAuditChangeEntity> builder) : base(db, builder)
    {
        
    }
}
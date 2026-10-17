using Airbnb.Audit.Domain.Models;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class ApartmentAuditRepository : MongoRepository<ApartmentAuditEntity>
{
    public ApartmentAuditRepository(MongoDbContext db) : base(db) { }
}
using Airbnb.Audit.Domain.Models;

namespace Airbnb.Audit.Infrastructure.Database.Repositories;

public class BookingAuditRepository : MongoRepository<BookingAuditEntity>
{
    public BookingAuditRepository(MongoDbContext db) : base(db) { }
}
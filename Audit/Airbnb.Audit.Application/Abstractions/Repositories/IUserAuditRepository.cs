using Airbnb.Audit.Domain.Models;

namespace Airbnb.Audit.Application.Abstractions.Repositories;

public interface IUserAuditRepository : IRepository<UserAuditChangeEntity>
{
    
}
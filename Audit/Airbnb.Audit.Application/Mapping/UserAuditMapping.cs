using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Domain.Models;
using Mapster;

namespace Airbnb.Audit.Application.Mapping;

public class UserAuditMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserAuditChangeEntity => UserAuditDto
        config.NewConfig<UserAuditEntity, UserAuditDto>();
    }
}
using Airbnb.Audit.Application.DTOs;
using Airbnb.Audit.Domain.Models;
using Mapster;

namespace Airbnb.Audit.Application.Mapping;

public class UserAuditMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserAuditChangeEntity => UserAuditDto
        config.NewConfig<UserAuditChangeEntity, UserAuditDto>();
    }
}
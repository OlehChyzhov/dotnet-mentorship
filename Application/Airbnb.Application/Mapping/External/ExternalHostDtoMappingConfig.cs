using Airbnb.Application.DTOs.External;
using Airbnb.Domain.Models;
using Mapster;

namespace Airbnb.Application.Mapping.External;

public class ExternalHostDtoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ExternalHostDto => User
        config.NewConfig<ExternalHostDto, User>()
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.ExternalUserId, src => src.ExternalId);
    }
}

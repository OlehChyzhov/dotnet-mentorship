using Airbnb.Application.DTOs.Authentication;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserRegisterRequest => Identity User
        config.NewConfig<UserRegisterDto, IdentityUser>();
    }
}
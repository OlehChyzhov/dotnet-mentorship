using Airbnb.Domain.Requests;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserRegisterRequest => Identity User
        config.NewConfig<UserRegisterRequest, IdentityUser>()
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.UserName, src => src.Email);
    }
}
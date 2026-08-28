using Airbnb.Application.DTOs.Authentication;
using Airbnb.Domain.Models;
using Mapster;

namespace Airbnb.Application.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // UserRegisterRequest => Identity User
        config.NewConfig<UserRegisterDto, User>();
    }
}
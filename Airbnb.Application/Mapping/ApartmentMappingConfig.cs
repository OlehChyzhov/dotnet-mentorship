using Airbnb.Application.DTOs;
using Airbnb.Domain.Models;
using Mapster;

namespace Airbnb.Application.Mapping;

public class ApartmentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Apartment => ApartmentDto
        config.NewConfig<Apartment, ApartmentDto>();
    }
}
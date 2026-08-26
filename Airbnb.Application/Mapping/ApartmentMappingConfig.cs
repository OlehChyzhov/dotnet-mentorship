using Airbnb.Application.DTOs;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Domain.Models;
using Mapster;

namespace Airbnb.Application.Mapping;

public class ApartmentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Apartment => ApartmentDto
        config.NewConfig<Apartment, ApartmentDto>();
        
        // CreateApartmentDto => Apartment
        config.NewConfig<CreateApartmentDto, Apartment>();
    }
}
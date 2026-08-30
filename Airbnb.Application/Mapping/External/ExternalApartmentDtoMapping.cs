using Airbnb.Application.DTOs.External;
using Airbnb.Domain.Models;
using Mapster;

namespace Airbnb.Application.Mapping.External;

public class ExternalApartmentDtoMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ExternalApartmentDto => Apartment
        config.NewConfig<ExternalApartmentDto, Apartment>()
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Map(dest => dest.ExternalId, src => src.ExternalId)
            .Ignore(dest => dest.OwnerId)
            .Ignore(dest => dest.Owner)
            .Ignore(dest => dest.Bookings);
    }
}

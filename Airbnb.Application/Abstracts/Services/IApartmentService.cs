using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;

namespace Airbnb.Application.Abstracts.Services;

public interface IApartmentService
{
    Task<(List<ApartmentDto> apartments, PagingMetaData metadata)> GetApartmentsAsync(ApartmentParameters parameters);

    Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto, string userId);
}
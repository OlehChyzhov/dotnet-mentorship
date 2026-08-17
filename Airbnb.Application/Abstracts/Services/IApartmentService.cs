using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;

namespace Airbnb.Application.Abstracts.Services;

public interface IApartmentService
{
    Task<ApartmentDto> GetApartmentByIdAsync(Guid id);
    
    Task<(List<ApartmentDto> apartments, PagingMetaData metadata)> GetApartmentsAsync(ApartmentPagingParamters query);

    Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto, string userId);
}
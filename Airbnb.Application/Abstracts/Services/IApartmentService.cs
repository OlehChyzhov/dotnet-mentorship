using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain;

namespace Airbnb.Application.Abstracts.Services;

public interface IApartmentService
{
    Task<ApartmentDto> GetApartmentByIdAsync(Guid id);
    
    Task<Result<PagedList<ApartmentDto>>> GetApartmentsAsync(ApartmentPagingParamters query);

    Task<ApartmentDto> CreateApartmentAsync(CreateApartmentDto dto, string userId);
}
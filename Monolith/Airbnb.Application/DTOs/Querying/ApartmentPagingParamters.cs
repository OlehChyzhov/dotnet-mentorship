using Airbnb.Contracts.Paging;

namespace Airbnb.Application.DTOs.Querying;

public record ApartmentPagingParamters : PagingParametersBase
{
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;

    public bool? IsListed { get; init; } = null;
}
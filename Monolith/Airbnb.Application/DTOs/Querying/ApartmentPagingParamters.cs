using Airbnb.Contracts.HTTP.Paging;

namespace Airbnb.Application.DTOs.Querying;

public record ApartmentPagingParamters : PagingPramsBase
{
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;

    public bool? IsListed { get; init; } = null;
}
namespace Airbnb.Application.DTOs.Querying.Filtering;

public record ApartmentQuery : BaseQuery
{
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;

    public bool? IsListed { get; init; } = null;
}
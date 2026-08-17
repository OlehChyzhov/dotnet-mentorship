namespace Airbnb.Application.DTOs.Querying;

public abstract record BaseQuery
{
    public int PageSize { get; init; } = 100;
    public int PageNumber { get; init; } = 1;
}
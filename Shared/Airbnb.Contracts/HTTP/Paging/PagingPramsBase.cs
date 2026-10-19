namespace Airbnb.Contracts.HTTP.Paging;

public abstract record PagingPramsBase
{
    public int PageSize { get; init; } = 100;
    public int PageNumber { get; init; } = 1;
}
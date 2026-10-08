namespace Airbnb.Contracts.Paging;

public abstract record PagingParametersBase
{
    public int PageSize { get; init; } = 100;
    public int PageNumber { get; init; } = 1;
}
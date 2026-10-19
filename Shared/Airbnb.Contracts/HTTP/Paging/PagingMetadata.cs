namespace Airbnb.Contracts.HTTP.Paging;

public class PagingMetadata
{
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
    
    public PagingMetadata(int totalCount, int pageNumber, int pageSize)
    {
        TotalCount = totalCount;
        CurrentPage = pageNumber;
        PageSize = pageSize;

        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPreviousPage = CurrentPage > 1;
        HasNextPage = CurrentPage < TotalPages;
    }
}
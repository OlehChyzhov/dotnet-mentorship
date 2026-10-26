namespace Airbnb.Shared.HTTP;

public class PagedResult<T>
{
    public PagingMetadata Metadata { get; init; }
    public List<T> Items { get; set; }
    
    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Metadata = new PagingMetadata(totalCount, pageNumber, pageSize);
        Items = items.ToList();
    }
}
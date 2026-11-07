using Airbnb.Shared.Common.Paging;

namespace Airbnb.Shared.Common;

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
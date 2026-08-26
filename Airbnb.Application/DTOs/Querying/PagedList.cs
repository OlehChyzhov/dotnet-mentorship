namespace Airbnb.Application.DTOs.Querying;

public class PagedList<T> : List<T>
{
    public PagingMetaData MetaData { get; init; }

    public PagedList(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        MetaData = new PagingMetaData()
        {
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
        
        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IEnumerable<T> source, int totalCount, int pageNumber, int pageSize)
    {
        var items = source.ToList();
        return new PagedList<T>(items, totalCount, pageNumber, pageSize);
    }
}
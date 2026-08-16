namespace Airbnb.Domain.Requests.Paging;

public abstract class BaseParameters
{
    public int PageSize { get; set; } = 100;
    public int PageNumber { get; set; } = 1;
}
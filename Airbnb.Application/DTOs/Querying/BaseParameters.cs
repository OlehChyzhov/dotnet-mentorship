namespace Airbnb.Application.DTOs.Querying;

public abstract class BaseParameters
{
    public int PageSize { get; set; } = 100;
    public int PageNumber { get; set; } = 1;
}
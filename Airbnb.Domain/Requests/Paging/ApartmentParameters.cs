namespace Airbnb.Domain.Requests.Paging;

public class ApartmentParameters : BaseParameters
{
    public DateTime? StartDate { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;

    public bool? IsListed { get; set; } = null;
}
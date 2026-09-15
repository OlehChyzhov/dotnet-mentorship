namespace Airbnb.Application.DTOs.Apartment;

public class ApartmentByProfitDto
{
    public string ApartmentTitle { get; set; } = string.Empty;
    public int NumberOfBookings { get; set; }
    public double TotalProfit { get; set; }
}
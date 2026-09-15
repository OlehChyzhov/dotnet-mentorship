namespace Airbnb.Application.DTOs.Apartment;

public class ApartmentCityAveragesDto
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ApartmentCount { get; set; }
    public double AveragePricePerNight { get; set; }
}
using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Apartment;

public class ApartmentTypeAveragesDto
{
    public ApartmentType Type { get; set; }
    public int ApartmentCount { get; set; }
    public double AveragePricePerNight { get; set; }
    public double MinPricePerNight { get; set; }
    public double MaxPricePerNight { get; set; }
}
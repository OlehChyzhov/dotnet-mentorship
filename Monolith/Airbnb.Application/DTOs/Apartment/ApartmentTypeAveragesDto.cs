using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Apartment;

public class ApartmentTypeAveragesDto
{
    public ApartmentType Type { get; set; }
    public int ApartmentCount { get; set; }
    public decimal AveragePricePerNight { get; set; }
    public decimal MinPricePerNight { get; set; }
    public decimal MaxPricePerNight { get; set; }
}
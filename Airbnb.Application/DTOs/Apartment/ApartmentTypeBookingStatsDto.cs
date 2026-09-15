using Airbnb.Domain.Enums;

namespace Airbnb.Application.DTOs.Apartment;

public class ApartmentTypeBookingStatsDto
{
    public ApartmentType Type { get; set; }
    public int TotalBookings { get; set; }
    public double AverageStayNights { get; set; }
}
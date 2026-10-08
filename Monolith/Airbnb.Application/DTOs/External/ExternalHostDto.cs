using System.Text.Json.Serialization;

namespace Airbnb.Application.DTOs.External;

public class ExternalHostDto
{
    [JsonPropertyName("Id")]
    public string ExternalId { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public List<ExternalApartmentDto> Apartments { get; set; } = new();
}
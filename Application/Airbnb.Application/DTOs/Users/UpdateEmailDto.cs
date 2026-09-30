namespace Airbnb.Application.DTOs.Users;

public record UpdateEmailDto
{
    public string NewEmail { get; init; } = string.Empty;
}

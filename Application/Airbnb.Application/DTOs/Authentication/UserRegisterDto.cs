using Airbnb.Domain.Constants;

namespace Airbnb.Application.DTOs.Authentication;

public record UserRegisterDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = Roles.Client;
}
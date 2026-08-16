using Airbnb.Domain.Constants;

namespace Airbnb.Application.DTOs.Authentication;

public class UserRegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.Client;
}
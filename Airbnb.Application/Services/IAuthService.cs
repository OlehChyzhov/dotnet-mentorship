using Airbnb.Domain;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Services;

public interface IAuthService
{
    public Task<IdentityResult> RegisterUserAsync(UserLoginRequest user);
}
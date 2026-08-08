using Airbnb.Application.Requests;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Services;

public interface IAuthService
{
    public Task<IdentityResult> RegisterUserAsync(UserLoginRequest user);

    public Task<(bool isSuccessful, string message)> LoginUserAsync(UserLoginRequest user);
}
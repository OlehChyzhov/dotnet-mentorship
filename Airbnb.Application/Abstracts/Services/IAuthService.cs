using Airbnb.Domain.Requests;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Services;

public interface IAuthService
{
    public Task<IdentityResult> RegisterUserAsync(UserRegisterRequest user);

    public Task<(bool isSuccessful, string message)> LoginUserAsync(UserLoginRequest user);
    
    public Task<string> GenerateJwtTokenAsync(UserLoginRequest user);
}
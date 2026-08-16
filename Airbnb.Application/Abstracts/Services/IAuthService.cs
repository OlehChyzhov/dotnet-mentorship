using Airbnb.Application.DTOs.Authentication;
using Airbnb.Domain;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Services;

public interface IAuthService
{
    public Task<IdentityResult> RegisterUserAsync(UserRegisterRequest user);

    public Task<Result<UserLoginRequest>> LoginUserAsync(UserLoginRequest user);
    
    public Task<string> GenerateJwtTokenAsync(UserLoginRequest user);
}
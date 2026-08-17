using Airbnb.Application.DTOs.Authentication;
using Airbnb.Domain;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Services;

public interface IAuthService
{
    public Task<IdentityResult> RegisterUserAsync(UserRegisterDto user);

    public Task<Result<UserLoginDto>> LoginUserAsync(UserLoginDto user);
    
    public Task<string> GenerateJwtTokenAsync(UserLoginDto user);
}
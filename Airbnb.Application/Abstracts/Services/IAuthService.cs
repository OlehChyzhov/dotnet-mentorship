using Airbnb.Domain;
using Airbnb.Domain.Requests;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Abstracts.Services;

public interface IAuthService
{
    public Task<IdentityResult> RegisterUserAsync(UserRegisterRequest user);

    public Task<Result<UserLoginRequest>> LoginUserAsync(UserLoginRequest user);
    
    public Task<string> GenerateJwtTokenAsync(UserLoginRequest user);
}
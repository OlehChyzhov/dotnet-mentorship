using Airbnb.Domain;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    
    public AuthService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserLoginRequest user)
    {
        IdentityUser identityUser = new IdentityUser()
        {
            UserName = user.Name,
            Email = user.Email,
        };
        
        return await _userManager.CreateAsync(identityUser, user.Password);
    }
}
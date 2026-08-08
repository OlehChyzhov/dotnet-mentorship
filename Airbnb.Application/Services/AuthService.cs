using Airbnb.Application.Requests;
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
            Email = user.Email,
        };
        
        return await _userManager.CreateAsync(identityUser, user.Password);
    }

    public async Task<(bool isSuccessful, string message)> LoginUserAsync(UserLoginRequest user)
    {
        IdentityUser? identityUser = await _userManager.FindByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return (false, "No user found");
        }
        
        if (await _userManager.CheckPasswordAsync(identityUser, user.Password))
        {
            return (true, "Login successful");
        }
        
        return (false, "Incorrect password");
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Airbnb.Application.Options;
using Airbnb.Domain.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IOptions<JwtOptions> _jwtOptions;
    
    public AuthService(
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtOptions =  jwtOptions;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegisterRequest user)
    {
        IdentityUser identityUser = new IdentityUser()
        {
            UserName = user.Email,
            Email = user.Email,
        };
        
        bool roleExists = await _roleManager.RoleExistsAsync(user.Role.ToString());
        if (roleExists)
        {
            var result = await _userManager.CreateAsync(identityUser, user.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            return await _userManager.AddToRoleAsync(identityUser, user.Role.ToString());
        }
        
        return IdentityResult.Failed(new  IdentityError()
        {
            Code = "RoleNotFound",
            Description = $"The role '{user.Role.ToString()}' does not exist"
        });
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

    public async Task<string> GenerateJwtTokenAsync(UserLoginRequest user)
    {
        IdentityUser? identityUser = await _userManager.FindByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return string.Empty;
        }
        
        IList<string> userRoles = await _userManager.GetRolesAsync(identityUser);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, user.Email),
        };
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        
        var securityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Value.Issuer, 
            audience: _jwtOptions.Value.Audience, 
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtOptions.Value.ExpirationMinutes),
            signingCredentials: credentials
        );
        
        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return token;
    }
}
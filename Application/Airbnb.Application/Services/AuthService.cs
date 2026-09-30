using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Authentication;
using Airbnb.Application.Options;
using Airbnb.Domain;
using Airbnb.Domain.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserManagementService _userManagementService;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly IMapper _mapper;
    
    public AuthService(
        IUserManagementService userManagementService,
        IOptions<JwtOptions> jwtOptions,
        IMapper mapper)
    {
        _userManagementService = userManagementService;
        _jwtOptions =  jwtOptions;
        _mapper = mapper;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegisterDto user)
    {
        User identityUser = _mapper.Map<UserRegisterDto, User>(user);
        
        bool roleExists = await _userManagementService.RoleExistsAsync(user.Role);
        if (roleExists)
        {
            var result = await _userManagementService.CreateUserAsync(identityUser, user.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            return await _userManagementService.AddUserToRoleAsync(identityUser, user.Role);
        }
        
        return IdentityResult.Failed(new  IdentityError()
        {
            Code = Constants.CodeRoleNotFound,
            Description = $"The role '{user.Role}' does not exist"
        });
    }

    public async Task<Result<UserLoginDto>> LoginUserAsync(UserLoginDto user)
    {
        User? identityUser = await _userManagementService.FindUserByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return "No user found";
        }
        
        if (await _userManagementService.CheckPasswordAsync(identityUser, user.Password))
        {
            return user;
        }

        return "Incorrect password";
    }

    public async Task<string> GenerateJwtTokenAsync(UserLoginDto user)
    {
        User? identityUser = await _userManagementService.FindUserByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return string.Empty;
        }
        
        IList<string> userRoles = await _userManagementService.GetRolesAsync(identityUser);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
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
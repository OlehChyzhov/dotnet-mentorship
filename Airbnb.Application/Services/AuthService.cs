using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.Options;
using Airbnb.Domain;
using Airbnb.Domain.Requests;
using Airbnb.Domain.Requests.Authentication;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly IMapper _mapper;
    
    public AuthService(
        IUserService userService,
        IOptions<JwtOptions> jwtOptions,
        IMapper mapper)
    {
        _userService = userService;
        _jwtOptions =  jwtOptions;
        _mapper = mapper;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegisterRequest user)
    {
        IdentityUser identityUser = _mapper.Map<UserRegisterRequest, IdentityUser>(user);
        
        bool roleExists = await _userService.RoleExistsAsync(user.Role);
        if (roleExists)
        {
            var result = await _userService.CreateUserAsync(identityUser, user.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            return await _userService.AddUserToRoleAsync(identityUser, user.Role);
        }
        
        return IdentityResult.Failed(new  IdentityError()
        {
            Code = Constants.CodeRoleNotFound,
            Description = $"The role '{user.Role}' does not exist"
        });
    }

    public async Task<Result<UserLoginRequest>> LoginUserAsync(UserLoginRequest user)
    {
        IdentityUser? identityUser = await _userService.FindUserByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return "No user found";
        }
        
        if (await _userService.CheckPasswordAsync(identityUser, user.Password))
        {
            return user;
        }

        return "Incorrect password";
    }

    public async Task<string> GenerateJwtTokenAsync(UserLoginRequest user)
    {
        IdentityUser? identityUser = await _userService.FindUserByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return string.Empty;
        }
        
        IList<string> userRoles = await _userService.GetRolesAsync(identityUser);

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
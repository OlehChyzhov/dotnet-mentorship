using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.Options;
using Airbnb.Domain.Requests;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Airbnb.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly IMapper _mapper;
    
    public AuthService(
        IUserRepository userRepository,
        IOptions<JwtOptions> jwtOptions,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtOptions =  jwtOptions;
        _mapper = mapper;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegisterRequest user)
    {
        IdentityUser identityUser = _mapper.Map<UserRegisterRequest, IdentityUser>(user);
        
        bool roleExists = await _userRepository.RoleExistsAsync(user.Role.ToString());
        if (roleExists)
        {
            var result = await _userRepository.CreateUserAsync(identityUser, user.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            return await _userRepository.AddUserToRoleAsync(identityUser, user.Role.ToString());
        }
        
        return IdentityResult.Failed(new  IdentityError()
        {
            Code = "RoleNotFound",
            Description = $"The role '{user.Role.ToString()}' does not exist"
        });
    }

    public async Task<(bool isSuccessful, string message)> LoginUserAsync(UserLoginRequest user)
    {
        IdentityUser? identityUser = await _userRepository.FindUserByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return (false, "No user found");
        }
        
        if (await _userRepository.CheckPasswordAsync(identityUser, user.Password))
        {
            return (true, "Login successful");
        }
        
        return (false, "Incorrect password");
    }

    public async Task<string> GenerateJwtTokenAsync(UserLoginRequest user)
    {
        IdentityUser? identityUser = await _userRepository.FindUserByEmailAsync(user.Email);
        if (identityUser == null)
        {
            return string.Empty;
        }
        
        IList<string> userRoles = await _userRepository.GetRolesAsync(identityUser);

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
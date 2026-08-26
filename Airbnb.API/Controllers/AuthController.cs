using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        IdentityResult result = await _authService.RegisterUserAsync(userRegisterDto);

        if (result.Succeeded)
        {
            return Ok("User created successfully");
        }
        
        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var result = await _authService.LoginUserAsync(userLoginDto);

        if (result.IsSuccessful)
        {
            string token = await _authService.GenerateJwtTokenAsync(userLoginDto);
            return Ok(token);
        }
        
        return BadRequest(result.Message);
    }
}
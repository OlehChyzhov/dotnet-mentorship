using Airbnb.Application.Services;
using Airbnb.Domain.Requests;
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
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest userRegisterRequest)
    {
        IdentityResult result = await _authService.RegisterUserAsync(userRegisterRequest);

        if (result.Succeeded)
        {
            return Ok("User created successfully");
        }
        
        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
    {
        var result = await _authService.LoginUserAsync(userLoginRequest);

        if (result.isSuccessful)
        {
            string token = await _authService.GenerateJwtTokenAsync(userLoginRequest);
            return Ok(token);
        }
        
        return BadRequest(result.message);
    }
}
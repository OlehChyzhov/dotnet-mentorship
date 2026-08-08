using Airbnb.Application.Requests;
using Airbnb.Application.Services;
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
    public async Task<IActionResult> Register([FromBody] UserLoginRequest userLoginRequest)
    {
        IdentityResult result = await _authService.RegisterUserAsync(userLoginRequest);

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
            return Ok(result.message);
        }
        
        return BadRequest(result.message);
    }
}
using Airbnb.Application.Abstracts.Broker;
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
    private readonly IEventPublisher _eventPublisher;
    
    public AuthController(IAuthService authService, IEventPublisher eventPublisher)
    {
        _authService = authService;
        _eventPublisher = eventPublisher;
    }

    [HttpGet("rabbitmq-test")]
    public async Task<IActionResult> Test()
    {
        await _eventPublisher.PublishAsync<string>();
        return Ok();
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
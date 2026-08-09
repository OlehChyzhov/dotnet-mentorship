using Airbnb.Application.Services;
using Airbnb.Domain.Requests;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<UserRegisterRequest> _registerValidator;
    private readonly IValidator<UserLoginRequest> _loginValidator;
    
    public AuthController(
        IAuthService authService,
        IValidator<UserRegisterRequest> registerValidator,
        IValidator<UserLoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest userRegisterRequest)
    {
        ValidationResult validationResult = await _registerValidator.ValidateAsync(userRegisterRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
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
        ValidationResult validationResult = await _loginValidator.ValidateAsync(userLoginRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var result = await _authService.LoginUserAsync(userLoginRequest);

        if (result.isSuccessful)
        {
            string token = await _authService.GenerateJwtTokenAsync(userLoginRequest);
            return Ok(token);
        }
        
        return BadRequest(result.message);
    }
}
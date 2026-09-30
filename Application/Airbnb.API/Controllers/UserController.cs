using System.Security.Claims;
using Airbnb.Application;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Users;
using Airbnb.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(Roles = $"{Roles.Client}, {Roles.Host}")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut("update/email")]
    public async Task<IActionResult> UpdateUserEmailAsync([FromBody] UpdateEmailDto updateEmailDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _userService.UpdateEmailAsync(userId, updateEmailDto.NewEmail);
        if (result.Succeeded)
        {
            return Ok("User email updated!");
        }

        if (result.Errors.Any(e => e.Code == Constants.CodeUserNotFound))
        {
            return NotFound("User does not exist");
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUserAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _userService.DeleteUserAsync(userId);

        if (result.Succeeded)
        {
            return Ok("User deleted!");
        }
        
        return BadRequest(result.Errors);
    }
}

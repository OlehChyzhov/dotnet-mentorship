using System.Security.Claims;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Domain.Constants;
using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(Roles = $"{Roles.Client}, {Roles.Host}")]
public class UserManagementController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    
    public UserManagementController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpPut("update/email")] 
    public async Task<IActionResult> UpdateUserEmailAsync()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email)!;

        var user = await _userManagementService.FindUserByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound("User does not exist");
        }
        
        var result = await _userManagementService.UpdateEmailAsync(user,  userEmail);
        if (result.Succeeded)
        {
            return Ok("User email updated!");
        }
        
        return BadRequest(result.Errors);
    }
}
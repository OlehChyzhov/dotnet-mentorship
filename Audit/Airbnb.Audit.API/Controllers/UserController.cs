using System.Text.Json;
using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.Querying;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Audit.API.Controllers;

[Route("users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserAuditService _userAuditService;
    
    public UserController(IUserAuditService service)
    {
        _userAuditService = service;
    }

    [HttpGet("{auditId}")]
    public async Task<IActionResult> GetAuditByIdAsync(string auditId)
    {
        var result = await _userAuditService.GetAuditByIdAsync(auditId);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Value);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAuditsAsync([FromQuery] UserAuditFilter parameters)
    {
        var result = await _userAuditService.GetAuditsPaged(parameters);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Value);
    }
}
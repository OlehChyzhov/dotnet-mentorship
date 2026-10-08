using System.Text.Json;
using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.Querying;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Audit.API.Controllers;

[Route("api/user-audits")]
[ApiController]
public class UserAuditsController : ControllerBase
{
    private readonly IUserAuditService _userAuditService;
    
    public UserAuditsController(IUserAuditService service)
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
    public async Task<IActionResult> GetAllAuditsAsync([FromQuery] UserAuditPagingParameters parameters)
    {
        var result = await _userAuditService.GetAuditsPaged(parameters);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value!.MetaData));
        
        return Ok(result.Value);
    }
}
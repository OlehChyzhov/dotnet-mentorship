using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.Querying;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Audit.API.Controllers;

[Route("api/apartment-audits")]
[ApiController]
public class ApartmentController : ControllerBase
{
    private readonly IApartmentAuditService _apartmentAuditService;
    
    public ApartmentController(IApartmentAuditService service)
    {
        _apartmentAuditService = service;
    }

    [HttpGet("{auditId}")]
    public async Task<IActionResult> GetAuditByIdAsync(string auditId)
    {
        var result = await _apartmentAuditService.GetAuditByIdAsync(auditId);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Value);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAuditsAsync([FromQuery] ApartmentAuditFilter parameters)
    {
        var result = await _apartmentAuditService.GetAuditsPaged(parameters);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Value);
    }
}
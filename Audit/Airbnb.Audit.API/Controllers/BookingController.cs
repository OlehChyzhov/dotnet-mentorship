using Airbnb.Audit.Application.Abstractions.Services;
using Airbnb.Audit.Application.Querying;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Audit.API.Controllers;

[Route("api/booking-audits")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingAuditService _bookingAuditService;
    
    public BookingController(IBookingAuditService service)
    {
        _bookingAuditService = service;
    }

    [HttpGet("{auditId}")]
    public async Task<IActionResult> GetAuditByIdAsync(string auditId)
    {
        var result = await _bookingAuditService.GetAuditByIdAsync(auditId);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Value);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAuditsAsync([FromQuery] BookingAuditFilter parameters)
    {
        var result = await _bookingAuditService.GetAuditsPaged(parameters);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Value);
    }
}
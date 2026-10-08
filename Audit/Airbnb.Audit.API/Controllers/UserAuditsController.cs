using Airbnb.Audit.Application.Abstractions.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Audit.API.Controllers;

[Route("api/user-audits")]
[ApiController]
public class UserAuditsController : ControllerBase
{
    private readonly IUserAuditRepository _repository;
    
    public UserAuditsController(IUserAuditRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllAudits()
    {
        return Ok(await _repository.GetAllAsync());
    }
}
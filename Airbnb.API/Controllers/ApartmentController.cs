using System.Security.Claims;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Application.Services;
using Airbnb.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Route("api")]
public class ApartmentController : ControllerBase
{
    private readonly ApartmentService _apartmentService;
    
    public ApartmentController(ApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }
    
    [Authorize(Roles = $"{Roles.Client}, {Roles.Host}")]
    [HttpGet("apartments")]
    public async Task<IActionResult> GetAllApartments([FromQuery] ApartmentParameters parameters)
    {
        var apartments = await _apartmentService.GetApartmentsAsync(parameters);
        return Ok(apartments.apartments);
    }

    [Authorize(Roles = $"{Roles.Host}")]
    [HttpPost("apartments")]
    public async Task<IActionResult> CreateApartmentAsync([FromBody] CreateApartmentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        var createdApartment = await _apartmentService.CreateApartmentAsync(dto, userId);
        return Ok(createdApartment);
    }
}
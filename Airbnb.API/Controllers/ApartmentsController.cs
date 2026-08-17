using System.Security.Claims;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Route("api/apartments")]
public class ApartmentsController : ControllerBase
{
    private readonly IApartmentService _apartmentService;
    
    public ApartmentsController(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    [HttpGet("{apartmentId:guid}")]
    [Authorize(Roles = $"{Roles.Client}, {Roles.Host}")]
    public async Task<IActionResult> GetApartmentById(Guid apartmentId)
    {
        var apartment = await _apartmentService.GetApartmentByIdAsync(apartmentId);
        return Ok(apartment);
    }
    
    [HttpGet]
    [Authorize(Roles = $"{Roles.Client}, {Roles.Host}")]
    public async Task<IActionResult> GetAllApartments([FromQuery] ApartmentQuery query)
    {
        var apartments = await _apartmentService.GetApartmentsAsync(query);
        return Ok(apartments.apartments);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Host}")]
    public async Task<IActionResult> CreateApartmentAsync([FromBody] CreateApartmentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        var createdApartment = await _apartmentService.CreateApartmentAsync(dto, userId);
        return CreatedAtAction(nameof(GetApartmentById), new { apartmentId = createdApartment.Id }, createdApartment);
    }
}
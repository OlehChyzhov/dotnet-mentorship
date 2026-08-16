using Airbnb.Application.DTOs.Apartment;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Application.Services;
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
    
    [Authorize(Roles = "Client,Host")]
    [HttpGet("apartments")]
    public async Task<IActionResult> GetAllApartments([FromQuery] ApartmentParameters parameters)
    {
        var apartments = await _apartmentService.GetApartmentsAsync(parameters);
        return Ok(apartments);
    }

    [Authorize(Roles = "Host")]
    [HttpPost("apartments")]
    public async Task<IActionResult> CreateApartmentAsync([FromBody] CreateApartmentDto dto)
    {
        var createdApartment = await _apartmentService.CreateApartmentAsync(dto);
        return Ok(createdApartment);
    }
}
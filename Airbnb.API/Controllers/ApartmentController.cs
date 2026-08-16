using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Authorize(Roles = "Client")]
[Route("api")]
public class ApartmentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    
    public ApartmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // TODO: Add services that return dtos
    // TODO: Create booking repository
    // TODO: Use services for filtering and mapping returning the dtos
    // TODO: implement controllers with the services adding paging info in the headers
    [HttpGet("apartments")]
    public async Task<ActionResult<List<Apartment>>> GetAllApartments([FromQuery] ApartmentParameters parameters)
    {
        var apartments = await _unitOfWork.Apartments.GetApartmentsPagedAsync(parameters);
        return Ok(apartments);
    }
}
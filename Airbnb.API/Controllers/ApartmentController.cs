using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Authorize(Roles = "Client")]
[Route("api/apartments")]
public class ApartmentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    
    public ApartmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult GetAllApartments()
    {
        return Ok();
    }
}
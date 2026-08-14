using Airbnb.Application.Abstracts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult GetUserBookings()
    {
        return Ok();
    }
    
    public IActionResult CreateBooking()
    {
        return Ok();
    }
}
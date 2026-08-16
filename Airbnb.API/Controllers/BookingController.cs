using System.Security.Claims;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[Authorize(Roles = "Client,Host")]
[ApiController]
[Route("api")]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }
    
    [HttpGet("bookings")]
    public async Task<IActionResult> GetUserBookings([FromQuery] BookingParameters parameters)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var bookings = await _bookingService.GetBookingsAsync(parameters, userId);
        return Ok(bookings);
    }

    [HttpPost("bookings")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var createdBooking = await _bookingService.CreateBookingAsync(dto, userId);
        return Ok(createdBooking);
    }
}
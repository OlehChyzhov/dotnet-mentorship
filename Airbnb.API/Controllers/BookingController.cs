using System.Security.Claims;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Application.Services;
using Airbnb.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[Authorize(Roles = $"{Roles.Client}, {Roles.Host}")]
[ApiController]
[Route("api")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    
    [HttpGet("bookings")]
    public async Task<IActionResult> GetUserBookings([FromQuery] BookingParameters parameters)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var bookings = await _bookingService.GetBookingsAsync(parameters, userId);
        return Ok(bookings.bookings);
    }

    [HttpPost("bookings")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var createdBooking = await _bookingService.CreateBookingAsync(dto, userId);
        return Ok(createdBooking);
    }
}
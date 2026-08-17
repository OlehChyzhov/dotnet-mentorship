using System.Security.Claims;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.DTOs.Booking;
using Airbnb.Application.DTOs.Querying.Filtering;
using Airbnb.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize(Roles = $"{Roles.Client}, {Roles.Host}")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBookingById(Guid bookingId)
    {
        var booking = await _bookingService.GetBookingByIdAsync(bookingId);
        return Ok(booking);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserBookings([FromQuery] BookingPagingParameters query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var bookings = await _bookingService.GetBookingsAsync(query, userId);
        return Ok(bookings.bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var createdBooking = await _bookingService.CreateBookingAsync(dto, userId);
        return CreatedAtAction(nameof(GetBookingById), new { bookingId = createdBooking.Id }, createdBooking);
    }
}
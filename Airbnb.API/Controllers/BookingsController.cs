using System.Security.Claims;
using System.Text.Json;
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
        var result = await _bookingService.GetBookingByIdAsync(bookingId);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Value);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserBookings([FromQuery] BookingPagingParameters query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _bookingService.GetBookingsAsync(query, userId);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.Value!.MetaData));
        
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _bookingService.CreateBookingAsync(dto, userId);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }
        
        return CreatedAtAction(nameof(GetBookingById), new { bookingId = result.Value!.Id }, result.Value);
    }
}
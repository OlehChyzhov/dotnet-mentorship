using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Payment.API.Controllers;

[Route("payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentWebhookService _paymentWebhookService;
    private readonly IPaymentService _paymentService;

    public PaymentController(
        IPaymentWebhookService paymentWebhookService,
        IPaymentService paymentService)
    {
        _paymentWebhookService = paymentWebhookService;
        _paymentService = paymentService;
    }

    [HttpGet("{bookingId}/session-url")]
    public async Task<IActionResult> GetPaymentSessionUrl(Guid bookingId)
    {
        var result = await _paymentService.GetPaymentSessionUrlAsync(bookingId);
        if (!result.IsSuccessful)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Value);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        var result = await _paymentWebhookService.HandleWebhookAsync(json, Request.Headers["Stripe-Signature"].ToString());
        if (!result.IsSuccessful)
        {
            return BadRequest();
        }

        return Ok();
    }
}

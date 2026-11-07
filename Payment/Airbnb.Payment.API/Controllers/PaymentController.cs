using Airbnb.Payment.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Payment.API.Controllers;

[Route("payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentWebhookService _paymentWebhookService;

    public PaymentController(IPaymentWebhookService paymentWebhookService)
    {
        _paymentWebhookService = paymentWebhookService;
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
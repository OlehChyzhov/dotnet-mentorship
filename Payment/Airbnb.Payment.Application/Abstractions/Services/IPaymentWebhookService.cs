using Airbnb.Shared.Common;

namespace Airbnb.Payment.Application.Abstractions.Services;

public interface IPaymentWebhookService
{
    Task<Result<bool>> HandleWebhookAsync(string json, string signature);
}
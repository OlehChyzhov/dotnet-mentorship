using Airbnb.Payment.Application.DTOs;
using Airbnb.Shared.Common;

namespace Airbnb.Payment.Application.Abstractions.Services;

public interface IPaymentService
{
    Task<Result<PaymentSessionUrlDto>> GetPaymentSessionUrlAsync(Guid bookingId);
}

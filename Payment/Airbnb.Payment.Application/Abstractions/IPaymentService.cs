using Airbnb.Payment.Application.DTOs;
using Airbnb.Shared.Common;
using Airbnb.Shared.Contracts.Messages;

namespace Airbnb.Payment.Application.Abstractions;

public interface IPaymentService
{
    Task<Result<PaymentSessionCreationDto>> CreatePaymentSessionAsync(CreatePaymentSessionMessage message);
}
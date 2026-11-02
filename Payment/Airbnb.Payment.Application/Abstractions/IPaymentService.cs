using Airbnb.Payment.Application.DTOs;
using Airbnb.Shared.Common;
using Airbnb.Shared.Contracts.Commands.Payment;

namespace Airbnb.Payment.Application.Abstractions;

public interface IPaymentService
{
    Result<PaymentSessionCreationDto> CreatePaymentSession(CreatePaymentSessionCommand command);
}
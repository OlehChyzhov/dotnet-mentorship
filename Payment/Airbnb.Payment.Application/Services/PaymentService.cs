using Airbnb.Payment.Application.Abstractions;
using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Application.Abstractions.Services;
using Airbnb.Payment.Application.DTOs;
using Airbnb.Payment.Domain;
using Airbnb.Shared.Common;
using MapsterMapper;

namespace Airbnb.Payment.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaymentSessionUrlDto>> GetPaymentSessionUrlAsync(Guid bookingId)
    {
        var transaction = await _unitOfWork.PaymentTransactions.GetByBookingIdAsync(bookingId);
        if (transaction == null)
        {
            return "Payment transaction for booking was not found";
        }

        if (string.IsNullOrEmpty(transaction.StripePaymentSessionUrl))
        {
            return "Payment session is not created yet";
        }

        if (transaction.TransactionStatus != PaymentTransactionStatus.Pending
            || transaction.PaymentSessionStatus == PaymentSessionStatus.Invalid)
        {
            return "Payment session is no longer valid";
        }

        return _mapper.Map<PaymentTransaction, PaymentSessionUrlDto>(transaction);
    }
}

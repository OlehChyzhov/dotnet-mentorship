using Airbnb.Payment.Domain;

namespace Airbnb.Payment.Application.Abstractions.Repositories;

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction>
{
    Task<PaymentTransaction?> GetByBookingIdAsync(Guid bookingId);
}

using Airbnb.Payment.Application.Abstractions.Repositories;
using Airbnb.Payment.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Payment.Infrastructure.Database.Repositories;

public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
{
    public PaymentTransactionRepository(ApplicationDbContext context) : base(context) {}

    public async Task<PaymentTransaction?> GetByBookingIdAsync(Guid bookingId)
    {
        return await DbSet.FirstOrDefaultAsync(transaction => transaction.BookingId == bookingId);
    }
}

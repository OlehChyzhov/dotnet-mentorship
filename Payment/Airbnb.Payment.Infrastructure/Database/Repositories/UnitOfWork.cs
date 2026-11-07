using Airbnb.Payment.Application.Abstractions.Repositories;

namespace Airbnb.Payment.Infrastructure.Database.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IPaymentTransactionRepository PaymentTransactions { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IPaymentTransactionRepository paymentTransactions)
    {
        _context = context;
        PaymentTransactions = paymentTransactions;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

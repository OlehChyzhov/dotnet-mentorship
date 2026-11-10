namespace Airbnb.Payment.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    IPaymentTransactionRepository PaymentTransactions { get; }

    Task<int> SaveChangesAsync();
}

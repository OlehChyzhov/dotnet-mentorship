namespace Airbnb.Application.Abstracts.Repositories;

public interface IUnitOfWork : IDisposable
{
    IApartmentRepository Apartments { get; }
    IBookingRepository Bookings { get; }

    Task StartTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    
    Task<int> SaveChangesAsync();
}
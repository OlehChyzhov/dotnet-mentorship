namespace Airbnb.Application.Abstracts.Repositories;

public interface IUnitOfWork : IDisposable
{
    IApartmentRepository Apartments { get; }
    IBookingRepository Bookings { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
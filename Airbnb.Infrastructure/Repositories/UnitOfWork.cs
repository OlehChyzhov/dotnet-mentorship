using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Domain.Models;

namespace Airbnb.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IApartmentRepository Apartments { get; }
    public IBookingRepository Bookings { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IApartmentRepository apartments,
        IBookingRepository bookings)
    {
        _context = context;
        Apartments = apartments;
        Bookings = bookings;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
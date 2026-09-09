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

    public async Task StartTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
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
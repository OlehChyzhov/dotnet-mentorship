using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Domain.Models;

namespace Airbnb.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) {}
}
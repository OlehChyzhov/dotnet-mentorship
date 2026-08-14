using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Domain.Models;

namespace Airbnb.Infrastructure.Repositories;

public class ApartmentRepository : Repository<Apartment>, IApartmentRepository
{
    public ApartmentRepository(ApplicationDbContext context) : base(context) {}
}
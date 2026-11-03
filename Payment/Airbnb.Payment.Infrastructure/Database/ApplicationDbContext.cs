using Airbnb.Payment.Domain;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Payment.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
}
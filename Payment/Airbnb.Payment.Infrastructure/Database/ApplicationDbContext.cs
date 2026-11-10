using Airbnb.Payment.Domain;
using Airbnb.Payment.Infrastructure.Database.Configurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Payment.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new PaymentTransactionConfiguration());
        
        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}

using Airbnb.Infrastructure.Saga;
using Airbnb.Infrastructure.Saga.BookingSaga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.Infrastructure.Database.Configurations;

public class SagaInstanceConfiguration : IEntityTypeConfiguration<BookingPaymentStateMachineInstance>
{
    public void Configure(EntityTypeBuilder<BookingPaymentStateMachineInstance> builder)
    {
        builder.HasKey(s => s.CorrelationId);
    }
}
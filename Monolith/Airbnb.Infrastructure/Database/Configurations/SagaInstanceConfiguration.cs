using Airbnb.Infrastructure.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.Infrastructure.Database.Configurations;

public class SagaInstanceConfiguration : IEntityTypeConfiguration<MockSagaStateMachineInstance>
{
    public void Configure(EntityTypeBuilder<MockSagaStateMachineInstance> builder)
    {
        builder.HasKey(s => s.CorrelationId);
    }
}
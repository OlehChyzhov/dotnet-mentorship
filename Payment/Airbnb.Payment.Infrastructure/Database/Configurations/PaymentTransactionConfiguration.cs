using Airbnb.Payment.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.Payment.Infrastructure.Database.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");
        builder.HasKey(transaction => transaction.Id);

        builder.HasIndex(transaction => transaction.BookingId).IsUnique();

        builder.Property(transaction => transaction.PaymentAmount)
            .HasPrecision(18, 2);

        builder.Property(transaction => transaction.StripeSessionId)
            .HasMaxLength(255);

        builder.Property(transaction => transaction.StripePaymentIntendId)
            .HasMaxLength(255);

        builder.Property(transaction => transaction.StripePaymentSessionUrl)
            .HasMaxLength(2048);

        builder.Property(transaction => transaction.TransactionStatus)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(transaction => transaction.RefundStatus)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(transaction => transaction.PaymentSessionStatus)
            .HasConversion<string>()
            .HasMaxLength(32);
    }
}

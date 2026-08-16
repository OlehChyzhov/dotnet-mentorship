using Airbnb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.Infrastructure.Configurations;

public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("Apartments");
        builder.HasKey(apartment => apartment.Id);

        builder.HasOne(apartment => apartment.Owner)
            .WithMany()
            .HasForeignKey(apartment => apartment.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(apartment => apartment.Bookings)
            .WithOne(booking => booking.Apartment)
            .HasForeignKey(booking => booking.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
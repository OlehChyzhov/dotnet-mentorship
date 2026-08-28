using Airbnb.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(booking => booking.Id);
        builder.HasIndex(booking => booking.ExternalId).IsUnique();

        builder.HasOne(booking => booking.Apartment)
            .WithMany(apartment => apartment.Bookings)
            .HasForeignKey(booking => booking.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(booking => booking.Client)
            .WithMany()
            .HasForeignKey(booking => booking.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
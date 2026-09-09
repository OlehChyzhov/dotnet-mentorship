using Airbnb.Domain.Models;
using Airbnb.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    
    public ApplicationDbContext(DbContextOptions options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfiguration(new IdentityRoleConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new ApartmentConfiguration());
        builder.ApplyConfiguration(new BookingConfiguration());
    }
}
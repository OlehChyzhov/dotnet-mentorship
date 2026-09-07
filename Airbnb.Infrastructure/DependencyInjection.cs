using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Domain.Models;
using Airbnb.Infrastructure.DataLoading;
using Airbnb.Infrastructure.Repositories;
using Airbnb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        
        // Services
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        
        // Helpers
        services.AddScoped<IExternalDataLoader, ExternalDataLoader>();
        
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Database (Identity)
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false; 
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
        
        
        
        return services;
    }
}
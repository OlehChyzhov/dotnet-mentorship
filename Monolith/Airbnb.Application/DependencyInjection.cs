using Airbnb.Application.Abstractions.Services;
using Airbnb.Application.Mapping;
using Airbnb.Application.Metrics;
using Airbnb.Application.Options;
using Airbnb.Application.Services;
using Airbnb.Application.Services.Booking;
using Airbnb.Application.Validators;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Services
        services.AddScoped<IApartmentService, ApartmentService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        
        // Booking Service Decorator
        services.AddScoped<BookingService>();
        services.AddScoped<IBookingService>(sp => new LockingBookingService(
            sp.GetRequiredService<BookingService>(),
            sp.GetRequiredService<ICacheBasedOperationLocker>()));

        // Metrics
        services.AddSingleton<BusinessMetrics>();

        // Mapping
        TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
        var applicationAssembly = typeof(UserMappingConfig).Assembly;
        TypeAdapterConfig.GlobalSettings.Scan(applicationAssembly);
        services.AddMapster();

        // Fluent Validation
        services.AddValidatorsFromAssembly(typeof(UserLoginRequestValidator).Assembly);

        // Options
        services.AddOptions<JwtOptions>().BindConfiguration("JWT");
        services.AddOptions<DefaultUserOptions>().BindConfiguration("DefaultUserOptions");

        return services;
    }
}
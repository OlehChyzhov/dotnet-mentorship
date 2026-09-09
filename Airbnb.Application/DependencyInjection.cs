using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.Mapping;
using Airbnb.Application.Options;
using Airbnb.Application.Services;
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
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IAuthService, AuthService>();

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
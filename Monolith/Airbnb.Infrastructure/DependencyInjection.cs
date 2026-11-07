using Airbnb.Application.Abstractions.Repositories;
using Airbnb.Application.Abstractions.Services;
using Airbnb.Application.Abstracts.Identity;
using Airbnb.Domain.Models;
using Airbnb.Infrastructure.Database;
using Airbnb.Infrastructure.Database.Repositories;
using Airbnb.Infrastructure.DataLoading;
using Airbnb.Infrastructure.Identity;
using Airbnb.Infrastructure.Options;
using Airbnb.Infrastructure.Saga;
using Airbnb.Infrastructure.Saga.BookingSaga;
using Airbnb.Infrastructure.Saga.BookingSaga.Success;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Message Broker (MassTransit.RabbitMQ)
        var rabbitmqSection = configuration.GetSection("RabbitMq");
        services.Configure<MessageBrokerOptions>(rabbitmqSection);
        var rabbitmqOptions = rabbitmqSection.Get<MessageBrokerOptions>()!;
        
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddSagaStateMachine<BookingPaymentSagaStateMachine, BookingPaymentStateMachineInstance>()
                .EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<ApplicationDbContext>();
                    r.UseSqlServer();
                });
            
            config.AddConsumersFromNamespaceContaining<FinishBookingCommandHandler>();
            
            config.UsingRabbitMq((context, rabbitMqConfig) =>
            {
                rabbitMqConfig.Host(new Uri(rabbitmqOptions.HostUri), hostAddress =>
                {
                    hostAddress.Username(rabbitmqOptions.UserName);
                    hostAddress.Password(rabbitmqOptions.Password);
                });
                
                rabbitMqConfig.UseInMemoryOutbox();
                rabbitMqConfig.ConfigureEndpoints(context);
            });
        });
        
        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        
        // Services
        services.AddScoped<IIdentityService, IdentityService>();
        
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
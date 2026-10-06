using Airbnb.Application.Abstracts.Broker;
using Airbnb.Application.Abstracts.Identity;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.Options;
using Airbnb.Domain.Models;
using Airbnb.Infrastructure.Broker;
using Airbnb.Infrastructure.Database;
using Airbnb.Infrastructure.Database.Repositories;
using Airbnb.Infrastructure.DataLoading;
using Airbnb.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Airbnb.Infrastructure;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructureAsync(this IServiceCollection services, IConfiguration configuration)
    {
        // Message Broker (RabbitMQ)
        var rabbitmqSection = configuration.GetSection("RabbitMq");
        services.Configure<MessageBrokerOptions>(rabbitmqSection);
        var rabbitmqOptions = rabbitmqSection.Get<MessageBrokerOptions>()!;

        var factory = new ConnectionFactory()
        {
            HostName = rabbitmqOptions.HostName,
            Port = rabbitmqOptions.Port,
            UserName = rabbitmqOptions.UserName,
            Password = rabbitmqOptions.Password,
            ClientProvidedName = rabbitmqOptions.ClientProvidedName
        };
        
        IConnection connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: rabbitmqOptions.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        services.AddSingleton<IConnection>(connection);
        services.AddSingleton<IChannel>(channel);
        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
        
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
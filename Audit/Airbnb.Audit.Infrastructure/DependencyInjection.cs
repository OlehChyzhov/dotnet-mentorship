using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Domain.Enums;
using Airbnb.Audit.Domain.Models;
using Airbnb.Audit.Infrastructure.Database.Configurations;
using Airbnb.Audit.Infrastructure.Database.Repositories;
using Airbnb.Audit.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructureAsync(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB
        var mongoOptions = configuration.GetSection("MongoDb").Get<MongoDbOptions>()!;
        services.AddSingleton<IMongoClient>(config =>
        {
            return new MongoClient(configuration.GetConnectionString(name: "MongoDb"));
        });
        
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase(mongoOptions.DatabaseName);
        });
        
        BsonClassMap.RegisterClassMap(new UserAuditChangeEntityMap());
        
        services.AddScoped<IUserAuditRepository, UserAuditRepository>();
        
        return services;
    }
}
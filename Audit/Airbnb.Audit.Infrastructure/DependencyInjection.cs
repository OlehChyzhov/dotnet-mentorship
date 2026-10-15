using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Infrastructure.Database.Maps;
using Airbnb.Audit.Infrastructure.Database.Repositories;
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
        var mongoUrl = MongoUrl.Create(configuration.GetConnectionString("MongoDb"));
        services.AddSingleton<IMongoClient>(config =>
        {
            return new MongoClient(mongoUrl);
        });
        
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase(mongoUrl.DatabaseName);
        });
        
        BsonClassMap.RegisterClassMap(new UserAuditChangeEntityMap());
        
        services.AddScoped<IUserAuditRepository, UserAuditRepository>();
        
        return services;
    }
}
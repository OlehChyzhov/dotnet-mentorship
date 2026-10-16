using Airbnb.Audit.Application.Abstractions.Repositories;
using Airbnb.Audit.Infrastructure.Database;
using Airbnb.Audit.Infrastructure.Database.CollectionConfigs;
using Airbnb.Audit.Infrastructure.Database.DbConfigs;
using Airbnb.Audit.Infrastructure.Database.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Context
        services.AddSingleton<MongoDbContext>(sp =>
        {
            var mongoUrl = MongoUrl.Create(configuration.GetConnectionString("MongoDb"));
            var client = new MongoClient(mongoUrl);
            var db =  client.GetDatabase(mongoUrl.DatabaseName);
            
            return new MongoDbContext(db);
        });

        // MongoDB Configurations
        services.AddHostedService<MongoIndexConfig>();
        
        // Collection Configurations
        BsonClassMap.RegisterClassMap(new UserAuditChangeEntityConfigs());
        
        services.AddScoped<IUserAuditRepository, UserAuditRepository>();
        
        return services;
    }
}
using Airbnb.Audit.Infrastructure.Database;
using Airbnb.Audit.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Airbnb.Audit.Infrastructure;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructureAsync(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB
        services.AddOptions<MongoDbOptions>().Bind(configuration.GetSection("MongoDb"));
        services.AddSingleton<IMongoClient>(config =>
        {
            return new MongoClient(configuration.GetConnectionString("MongoDb"));
        });
        
        services.AddScoped<MongoDbContext>();
        
        return services;
    }
}
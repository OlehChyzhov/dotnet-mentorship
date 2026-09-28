using Airbnb.Application;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Airbnb.CLI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            await Console.Error.WriteLineAsync("Usage: dotnet run --project Airbnb.CLI -- <path-to-file>.json");
            return;
        }

        var filePath = args[0];

        Console.WriteLine($"Loading data from file {filePath}");

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        });
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);

        using var host = builder.Build();
        using var scope = host.Services.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<IExternalDataLoader>();
        var result = await dataLoader.LoadDataFromJsonFileAsync(filePath);

        Console.WriteLine(result.IsSuccessful ? result.Value : result.Message);
    }
}
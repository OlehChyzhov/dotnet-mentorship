using Airbnb.Audit.Application;
using Airbnb.Audit.Infrastructure;
using Airbnb.Contracts.Broker;
using Airbnb.Contracts.Messages;
using Airbnb.Messaging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Message Broker
await builder.Services.AddConsumers(builder.Configuration);

// DbContext, Repositories, UnitOfWork, etc.
await builder.Services.AddInfrastructureAsync(builder.Configuration);

// Services, Mapping, Validation
builder.Services.AddApplication();

// Default
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Airbnb API")
            .WithTheme(ScalarTheme.Default)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var allConsumers = app.Services.GetServices<IEventConsumer>();
foreach (var consumer in allConsumers)
{
    await consumer.StartAsync();
}

app.Run();
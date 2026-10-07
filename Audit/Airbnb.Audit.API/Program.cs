using Airbnb.Audit.Infrastructure;
using Airbnb.Messaging;
using Airbnb.Messaging.Broker.Consumer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext, Repositories, UnitOfWork, etc.
await builder.Services.AddInfrastructureAsync(builder.Configuration);

// Message Broker
await builder.Services.AddMessaging(builder.Configuration);

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

await app.Services.GetRequiredService<IEventConsumer>().StartAsync();

app.Run();
using System.Text.Json.Serialization;
using Airbnb.Audit.Application;
using Airbnb.Audit.Infrastructure;
using Airbnb.Audit.Infrastructure.Consumers;
using Airbnb.Contracts.Broker;
using Airbnb.Messaging;
using MassTransit;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Message Broker
builder.Services.AddRabbitMq(builder.Configuration, config =>
{
    config.AddConsumersFromNamespaceContaining<ApartmentMessageConsumer>();
});

// DbContext, Repositories, UnitOfWork, etc.
builder.Services.AddInfrastructure(builder.Configuration);

// Services, Mapping, Validation
builder.Services.AddApplication();

// Default
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

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

app.Run();
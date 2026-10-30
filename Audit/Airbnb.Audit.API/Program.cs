using System.Text.Json.Serialization;
using Airbnb.Audit.Application;
using Airbnb.Audit.Infrastructure;
using Airbnb.Audit.Infrastructure.Consumers;
using Airbnb.Shared;
using MassTransit;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;

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

// Logging
builder.Services.AddSerilog(config =>
{
    config.Enrich.FromLogContext();
    config.Enrich.WithMachineName();
    config.WriteTo.Console();

    string? uri = builder.Configuration["ElasticConfigurations:Uri"];
    if (string.IsNullOrEmpty(uri))
    {
        throw new InvalidOperationException("ElasticConfigurations:Uri is not configured");
    }
    
    config.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(uri)));
    config.Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);
});

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

app.UseSerilogRequestLogging();

app.Run();
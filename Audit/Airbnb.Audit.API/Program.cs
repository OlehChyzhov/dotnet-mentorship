using System.Text.Json.Serialization;
using Airbnb.Audit.API.Options;
using Airbnb.Audit.Application;
using Airbnb.Audit.Infrastructure;
using Airbnb.Audit.Infrastructure.Consumers;
using Airbnb.Shared;
using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
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
    string projectName = builder.Environment.ApplicationName.ToLowerInvariant();
    string environment = builder.Environment.EnvironmentName.ToLowerInvariant();

    config.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

    config.Enrich.FromLogContext();
    config.Enrich.WithMachineName();
    config.Enrich.WithProperty("Application", projectName);
    config.Enrich.WithProperty("Environment", environment);

    // Console
    config.WriteTo.Console();

    // Elasticsearch
    var elasticOptions = builder.Configuration.GetSection("ElasticConfigurations").Get<ElasticOptions>();
    if (string.IsNullOrEmpty(elasticOptions?.Uri))
    {
        throw new InvalidOperationException("ElasticConfigurations:Uri is not configured");
    }

    config.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticOptions.Uri))
    {
        IndexFormat = $"{projectName}-logs-{environment}-{{0:yyyy.MM.dd}}",
        AutoRegisterTemplate = true
    });
});

// Open Telemetry and Prometheus
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Microsoft.AspNetCore.Hosting");
        metrics.AddMeter("System.Runtime");
        metrics.AddPrometheusExporter();
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

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, ex) =>
        httpContext.Request.Path.StartsWithSegments("/metrics") ? LogEventLevel.Verbose
        : ex != null || httpContext.Response.StatusCode > 499 ? LogEventLevel.Error
        : LogEventLevel.Information;
});
app.MapPrometheusScrapingEndpoint();

app.Run();
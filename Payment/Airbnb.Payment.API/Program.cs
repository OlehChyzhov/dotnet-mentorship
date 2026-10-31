using Airbnb.Payment.API.Options;
using Serilog;
using Serilog.Events;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Airbnb.Payment.API")
            .WithTheme(ScalarTheme.Default)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http);
    });
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, ex) =>
        httpContext.Request.Path.StartsWithSegments("/metrics") ? LogEventLevel.Verbose
        : ex != null || httpContext.Response.StatusCode > 499 ? LogEventLevel.Error
        : LogEventLevel.Information;
});

app.MapPrometheusScrapingEndpoint();

app.Run();
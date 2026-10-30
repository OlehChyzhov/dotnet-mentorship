using Airbnb.Gateway.Options;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var proxyConfig = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy().LoadFromConfig(proxyConfig);

builder.Services.AddHealthChecks();

builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions.AddSlidingWindowLimiter("SlidingWindows", options =>
    {
        options.Window = TimeSpan.FromSeconds(15);
        options.SegmentsPerWindow = 3;
        options.PermitLimit = 15;
    });
});

// Logging
builder.Services.AddSerilog(config =>
{
    string projectName = builder.Environment.ApplicationName.ToLowerInvariant();
    string environment = builder.Environment.EnvironmentName.ToLowerInvariant();

    config.Enrich.FromLogContext();
    config.Enrich.WithMachineName();
    config.Enrich.WithProperty("Application", projectName);
    config.Enrich.WithProperty("Environment", environment);

    // Console
    config.WriteTo.Console();

    // Elastic Search
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapReverseProxy();
app.MapHealthChecks("/health");

app.UseRateLimiter();
app.UseSerilogRequestLogging();

app.Run();
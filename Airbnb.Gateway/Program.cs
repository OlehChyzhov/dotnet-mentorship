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
using System.Security.Claims;
using System.Text;
using Airbnb.API.Middleware;
using Airbnb.API.Options;
using Airbnb.Application;
using Airbnb.Infrastructure;
using Airbnb.Infrastructure.Database;
using Airbnb.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Middlewares
builder.Services.AddTransient<ValidationMiddleware>();

// Message Broker
builder.Services.AddRabbitMq(builder.Configuration);

// DbContext, Repositories, UnitOfWork, etc.
await builder.Services.AddInfrastructureAsync(builder.Configuration);

// Services, Mapping, Validation, etc.
builder.Services.AddApplication(builder.Configuration);

// JWT
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var hasNameIdentifier = context.Principal?.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier) ?? false;
                if (!hasNameIdentifier)
                {
                    context.Fail("Token is missing the required NameIdentifier claim.");
                }

                return Task.CompletedTask;
            }
        };
    });

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

    // TODO: Replace with explicit step in CI/CD using bundle
    // TODO: Add migration in docker-compose file
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Custom Middlewares
app.UseMiddleware<ValidationMiddleware>();

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
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var config = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy().LoadFromConfig(config);

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapReverseProxy();
app.MapHealthChecks("/health");

app.UseRateLimiter();

app.Run();
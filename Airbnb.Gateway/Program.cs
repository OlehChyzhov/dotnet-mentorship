var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var config = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy().LoadFromConfig(config);

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapReverseProxy();
app.MapHealthChecks("/health");

app.Run();
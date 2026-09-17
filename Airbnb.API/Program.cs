using System.Security.Claims;
using System.Text;
using Airbnb.API.Middleware;
using Airbnb.Application;
using Airbnb.Infrastructure;
using Airbnb.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Middlewares
builder.Services.AddTransient<ValidationMiddleware>();

// DbContext, Repositories, UnitOfWork, etc.
builder.Services.AddInfrastructure(builder.Configuration);

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

// Custom Middlewares
app.UseMiddleware<ValidationMiddleware>();

app.MapControllers();

app.Run();
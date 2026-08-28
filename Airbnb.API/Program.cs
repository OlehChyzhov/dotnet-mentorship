using System.Security.Claims;
using System.Text;
using Airbnb.API.Middleware;
using Airbnb.Application.Abstracts.Helpers;
using Airbnb.Application.Abstracts.Repositories;
using Airbnb.Application.Abstracts.Services;
using Airbnb.Application.Helpers;
using Airbnb.Application.Mapping;
using Airbnb.Application.Options;
using Airbnb.Application.Services;
using Airbnb.Application.Validators;
using Airbnb.Infrastructure;
using Airbnb.Infrastructure.Helpers;
using Airbnb.Infrastructure.Repositories;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Middlewares
builder.Services.AddTransient<ValidationMiddleware>();

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Services
builder.Services.AddScoped<IApartmentService, ApartmentService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Helpers
builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<IDataLoader, DataLoader>();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Database (Identity)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

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

// Mapping
TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
var applicationAssembly = typeof(UserMappingConfig).Assembly;
TypeAdapterConfig.GlobalSettings.Scan(applicationAssembly);
builder.Services.AddMapster();

// Fluent Validation
builder.Services.AddValidatorsFromAssembly(typeof(UserLoginRequestValidator).Assembly);

// Options
builder.Services.AddOptions<JwtOptions>().BindConfiguration("JWT");
builder.Services.AddOptions<DataFileOptions>().BindConfiguration("DataFile");

// Default
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataLoader = scope.ServiceProvider.GetRequiredService<IDataLoader>();
    await dataLoader.LoadDataFromJsonFileAsync();
}

if (app.Environment.IsDevelopment())
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
using Airbnb.API.Middleware;

namespace Airbnb.API.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ValidationMiddleware>();
    }
}
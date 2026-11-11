using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Airbnb.Shared.Web;

public record CachedResponse(int StatusCode, string Body);

public class Idempotent : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Idempotency-Key", out var key) || string.IsNullOrWhiteSpace(key))
        {
            context.Result = new BadRequestObjectResult("X-Idempotency-Key header is required");
            return;
        }
        
        var cache = context.HttpContext.RequestServices.GetRequiredService<HybridCache>();
        var idempotencyKey = BuildKey(context);
        
        // Using the key check if the data exists and if it doesn't just return the null and don't store it
        var cacheResult = await cache.GetOrCreateAsync(
            key: idempotencyKey,
            cacheMiss => ValueTask.FromResult<CachedResponse?>(null),
            new HybridCacheEntryOptions() { Flags = HybridCacheEntryFlags.DisableLocalCacheWrite | HybridCacheEntryFlags.DisableDistributedCacheWrite });

        // If the idempotency key has been seem before, exit here
        if (cacheResult is not null)
        {
            context.Result = new ContentResult()
            {
                StatusCode = cacheResult.StatusCode,
                Content = cacheResult.Body,
                ContentType = "application/json"
            };

            return;
        }
        
        var methodExecuted = await next(); // Execute Controller Method
        
        if (methodExecuted.Result is ObjectResult { Value: not null } objectResult && IsSuccessfulResult(objectResult))
        {
            // Serialization options that are currently configured in Program.cs
            // RequestServices is the DI container for the current request; An attribute can't use constructor injection
            var jsonOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
            var response = new CachedResponse(
                StatusCode: objectResult.StatusCode ?? StatusCodes.Status200OK,
                Body: JsonSerializer.Serialize(objectResult.Value, jsonOptions));
            
            await cache.SetAsync(idempotencyKey, response);
        }
    }

    private string BuildKey(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var endpoint = $"{httpContext.Request.Method}:{httpContext.Request.Path}";
        var idempotencyKey = httpContext.Request.Headers["X-Idempotency-Key"];
        
        return $"{userId}_{endpoint}_{idempotencyKey}";
    }

    private bool IsSuccessfulResult(ObjectResult objectResult)
    {
        // when object returned directly the status code is skipped
        objectResult.StatusCode ??= 200;
        
        return objectResult.StatusCode >= 200 && objectResult.StatusCode < 300;
    }
}
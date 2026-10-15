using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Airbnb.API.Middleware;

public class ValidationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Endpoint? endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        if (actionDescriptor is null)
        {
            await next(context);
            return;
        }

        // The [FromBody] parameter, if the action has one
        var bodyParameter =
            actionDescriptor.Parameters.FirstOrDefault(p => p.BindingInfo?.BindingSource == BindingSource.Body);

        if (bodyParameter is null)
        {
            await next(context);
            return;
        }

        // Buffer so the model binder can still read the body after the middleware
        context.Request.EnableBuffering();

        object? model = await context.Request.ReadFromJsonAsync(bodyParameter.ParameterType);
        context.Request.Body.Position = 0;

        if (model is null)
        {
            await next(context);
            return;
        }

        // Build IValidator<T> at runtime
        Type validatorType = typeof(IValidator<>).MakeGenericType(model.GetType());
        var validator = context.RequestServices.GetService(validatorType) as IValidator;

        if (validator is null)
        {
            await next(context);
            return;
        }

        var validationContext = new ValidationContext<object>(model);
        ValidationResult validationResult = await validator.ValidateAsync(validationContext);

        if (!validationResult.IsValid)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            await context.Response.WriteAsJsonAsync(new { errors });
            return;
        }

        await next(context);
    }
}

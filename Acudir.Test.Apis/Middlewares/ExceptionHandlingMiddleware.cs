using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Acudir.Test.Apis.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = false };
    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {

            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (status, title, type) = ex switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation error", "BadRequest"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found", "NotFound"),
            _ => (HttpStatusCode.InternalServerError, "Unexpected error", "InternalServerError")
        };

        var problem = new ProblemDetails
        {
            Title = title,
            Type = type,
            Status = (int)status,
            Detail = ex is ValidationException v ? string.Join(" | ", v.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")) : ex.Message,
            Instance = ctx.Request.Path
        };

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

        return ctx.Response.WriteAsync(JsonSerializer.Serialize(problem, _jsonOptions));
    }
}

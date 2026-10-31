using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FunkoAppi01.Middleware;

/// <summary>
/// Captura las excepciones no controladas de la tuberia de peticiones y las traduce a ProblemDetails.
/// </summary>
public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Solicitud invalida");
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Solicitud invalida", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Error interno",
                "Se ha producido un error inesperado");
        }
    }

    private static Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyModularStore.Shared.ErrorHandling.Handlers;

public class ServiceUnavailableExceptionHandler : IErrorHandler
{
    public async Task HandleAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = 503;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = 503,
            Title = "Service Unavailable",
            Detail = ex.Message
        });
    }
}

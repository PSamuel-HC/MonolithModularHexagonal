using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace MyModularStore.Shared.ErrorHandling.Handlers;

public class ValidationExceptionHandler : IErrorHandler
{
    public async Task HandleAsync(HttpContext context, Exception ex)
    {
        var ve = (ValidationException)ex;
        var errors = ve.Errors.Select(e => e.ErrorMessage).ToArray();

        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            status = 400,
            title = "Validation Failed",
            errors
        });
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyModularStore.Shared.ErrorHandling.Handlers
{
    public class NotFoundExceptionHandler : IErrorHandler
    {
        public async Task HandleAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 404,
                Title = "Not Found",
                Detail = ex.Message
            });
        }
    }
}

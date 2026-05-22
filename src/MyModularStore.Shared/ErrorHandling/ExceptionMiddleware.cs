using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyModularStore.Shared.ErrorHandling
{
    public class ExceptionMiddleware(
        RequestDelegate next,
        Dictionary<Type, IErrorHandler> handlers)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (handlers.TryGetValue(ex.GetType(), out var handler))
                {
                    await handler.HandleAsync(context, ex);
                }
                else
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = 500,
                        Title = "Internal Server Error",
                        Detail = ex.Message
                    });
                }
            }
        }
    }
}

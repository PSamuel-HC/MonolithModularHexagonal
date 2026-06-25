using Serilog.Context;

namespace MyModularStore.Orders.Infrastructure.Middleware
{
    public class CorrelationIdMiddleware(RequestDelegate next)
    {
        private const string HeaderName = "X-Correlation-ID";

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                                ?? Guid.NewGuid().ToString("N")[..8];

            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next(context);
            }
        }
    }
}

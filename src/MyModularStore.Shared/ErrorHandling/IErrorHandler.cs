using Microsoft.AspNetCore.Http;

namespace MyModularStore.Shared.ErrorHandling
{
    public interface IErrorHandler
    {
        Task HandleAsync(HttpContext context, Exception ex);
    }
}

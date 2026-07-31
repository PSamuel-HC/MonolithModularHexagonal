using Microsoft.AspNetCore.Http.HttpResults;

namespace MyModularStore.Customers.Operations
{
    public static class OperationEndpoints
    {
        public static void MapOperationEndpoints(this WebApplication app)
        {
            app.MapGroup("/api/operations")
                .WithTags("Operations")
                .MapGet("/{id}", GetOperation);
        }

        static Results<Ok<OperationRecord>, NotFound> GetOperation(string id, OperationStore store)
        {
            var op = store.Get(id);
            return op is null ? TypedResults.NotFound() : TypedResults.Ok(op);
        }
    }
}

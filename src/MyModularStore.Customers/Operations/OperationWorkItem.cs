namespace MyModularStore.Customers.Operations
{
    public record OperationWorkItem(
        string OperationId,
        int CustomerId,
        string? WebhookUrl);
}

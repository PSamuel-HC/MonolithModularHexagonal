namespace MyModularStore.Customers.Operations
{
    public record OperationRecord(
        string Id,
        OperationStatus Status,
        object? Result,
        string? Error,
        DateTime CreatedAt,
        DateTime? CompletedAt);
}

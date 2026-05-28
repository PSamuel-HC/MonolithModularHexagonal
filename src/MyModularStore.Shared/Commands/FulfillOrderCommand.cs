namespace MyModularStore.Shared.Commands
{
    public record FulfillOrderCommand
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
    }
}

namespace MyModularStore.Shared.Events
{
    public class OrderFulfilledEvent
    {
        public int OrderId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public DateTime FulfilledAt { get; init; }
    }
}

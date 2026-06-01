namespace MyModularStore.Shared.Events
{
    public class OrderFulfillmentRequestedEvent
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
    }
}

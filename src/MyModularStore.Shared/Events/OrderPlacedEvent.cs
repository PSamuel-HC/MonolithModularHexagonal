namespace MyModularStore.Shared.Events
{
    public class OrderPlacedEvent
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
        public DateTime PlacedAt { get; init; }
    }
}

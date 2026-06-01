namespace MyModularStore.Shared.Events
{
    public class EmailSendRequestedEvent
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
    }
}

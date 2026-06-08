namespace MyModularStore.Shared.Events
{
    public class EmailSentEvent
    {
        public int OrderId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
    }
}

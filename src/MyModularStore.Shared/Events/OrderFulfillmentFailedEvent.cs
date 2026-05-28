namespace MyModularStore.Shared.Events 
{ 
    public class OrderFulfillmentFailedEvent 
    { 
        public int OrderId { get; init; } 
        public string OrderNumber { get; init; } = string.Empty; 
        public string Reason { get; init; } = string.Empty; 
    } 
}

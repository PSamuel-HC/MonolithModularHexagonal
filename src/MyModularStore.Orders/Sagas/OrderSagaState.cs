using MassTransit;

namespace MyModularStore.Orders.Sagas
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public string OrderIdKey { get; set; } = string.Empty; // ← used for correlation
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime PlacedAt { get; set; }
    }
}

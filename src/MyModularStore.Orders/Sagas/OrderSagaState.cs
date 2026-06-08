using MassTransit;

namespace MyModularStore.Orders.Sagas
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = "Initial";
        public string OrderIdKey { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime PlacedAt { get; set; }
        public CompositeEventStatus ReadyToComplete { get; set; }
    }
}

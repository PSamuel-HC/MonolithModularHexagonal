using MassTransit;
using MyModularStore.Shared.Commands;
using MyModularStore.Shared.Events;

namespace MyModularStore.Orders.Sagas
{
    public class OrderSaga : MassTransitStateMachine<OrderSagaState>
    {
        public State Fulfilling { get; private set; } = null!;
        public State Completed { get; private set; } = null!;
        public State Failed { get; private set; } = null!;


        public Event<OrderPlacedEvent> OrderPlaced { get; private set; } = null!;
        public Event<OrderFulfilledEvent> OrderFulfilled { get; private set; } = null!;
        public Event<OrderFulfillmentFailedEvent> OrderFulfillmentFailed { get; private set; } = null!;

        public OrderSaga()
        {
            //Where to save Saga Event Status
            InstanceState(x => x.CurrentState);


            //Correlation
            Event(() => OrderPlaced, e =>
            {                      //Sagas                      //Broker
                e.CorrelateBy(state => state.OrderIdKey, ctx => ctx.Message.OrderId.ToString());
                e.SelectId(_ => NewId.NextGuid());
            });

            Event(() => OrderFulfilled, e =>
                e.CorrelateBy(state => state.OrderIdKey, ctx => ctx.Message.OrderId.ToString()));

            Event(() => OrderFulfillmentFailed, e =>
                e.CorrelateBy(state => state.OrderIdKey, ctx => ctx.Message.OrderId.ToString()));


            Initially(
                When(OrderPlaced)
                    .Then(ctx =>
                    {
                        ctx.Saga.OrderId = ctx.Message.OrderId;
                        ctx.Saga.OrderIdKey = ctx.Message.OrderId.ToString();
                        ctx.Saga.CustomerId = ctx.Message.CustomerId;
                        ctx.Saga.OrderNumber = ctx.Message.OrderNumber;
                        ctx.Saga.PlacedAt = ctx.Message.PlacedAt;
                        Console.WriteLine(
                            $"[Saga] Order #{ctx.Saga.OrderNumber} placed → sending fulfillment command.");
                    })
                    .Send(ctx => new FulfillOrderCommand
                    {
                        OrderId = ctx.Saga.OrderId,
                        CustomerId = ctx.Saga.CustomerId,
                        OrderNumber = ctx.Saga.OrderNumber
                    })
                    .TransitionTo(Fulfilling)
                );

            During(Fulfilling,
                When(OrderFulfilled)
                    .Then(ctx =>
                        Console.WriteLine(
                            $"[Saga] Order #{ctx.Saga.OrderNumber} fulfilled → Completed."))
                    .TransitionTo(Completed)
                    .Finalize(),

                When(OrderFulfillmentFailed)
                    .Then(ctx =>
                        Console.WriteLine(
                            $"[Saga] Order #{ctx.Saga.OrderNumber} failed → reason: {ctx.Message.Reason}"))
                    .TransitionTo(Failed)
                    .Finalize()
            );


            SetCompletedWhenFinalized();
        }
    }
}

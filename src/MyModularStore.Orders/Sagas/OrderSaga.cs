using MassTransit;
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
        public Event<EmailSentEvent> EmailSent { get; private set; } = null!;
        public Event<OrderFulfillmentFailedEvent> OrderFulfillmentFailed { get; private set; } = null!;
        public Event ReadyToComplete { get; private set; } = null!;



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

            Event(() => EmailSent, e =>
                e.CorrelateBy(state => state.OrderIdKey, ctx => ctx.Message.OrderId.ToString()));


            //Composite Event Definition
            CompositeEvent(() => ReadyToComplete,
                state => state.ReadyToComplete,
                OrderFulfilled, EmailSent);

            //Initial
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
                    .PublishAsync(ctx => ctx.Init<OrderFulfillmentRequestedEvent>(new
                    {
                        ctx.Saga.OrderId,
                        ctx.Saga.CustomerId,
                        ctx.Saga.OrderNumber
                    }))
                    .TransitionTo(Fulfilling)
                );


            //Event in Process
            During(Fulfilling,
                When(OrderFulfilled)
                    .Then(ctx => Console.WriteLine(
                        $"[Saga] Composite 1/2 — fulfilled #{ctx.Saga.OrderNumber} (waiting for email)")),

                When(EmailSent)
                    .Then(ctx => Console.WriteLine(
                        $"[Saga] Composite 1/2 — email sent #{ctx.Saga.OrderNumber} (waiting for fulfillment)")),

                When(ReadyToComplete)
                    .Then(ctx => Console.WriteLine(
                        $"[Saga] Composite 2/2 — both done #{ctx.Saga.OrderNumber} → Completed."))
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

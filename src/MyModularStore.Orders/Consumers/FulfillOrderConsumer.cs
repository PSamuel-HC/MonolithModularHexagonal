using MassTransit;
using MyModularStore.Shared.Commands;

namespace MyModularStore.Orders.Consumers
{
    public class FulfillOrderConsumer : IConsumer<FulfillOrderCommand>
    {
        public async Task Consume(ConsumeContext<FulfillOrderCommand> context)
        {

            Console.WriteLine($"[FulfillOrderConsumer] Fulfilling order #{context.Message.OrderNumber}");

            // Simulate work: reserve stock, generate shipping label
            await Task.Delay(500);

            Console.WriteLine($"[FulfillOrderConsumer] Order #{context.Message.OrderNumber} fulfilled.");

            // MassTransit ACKs automatically when Consume() returns without exception
            // If an exception is thrown → retry → then DLQ
        }
    }
}

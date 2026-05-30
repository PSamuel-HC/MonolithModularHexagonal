using MassTransit;
using MyModularStore.Shared.Commands;
using MyModularStore.Shared.Events;
using System.Collections.Concurrent;

namespace MyModularStore.Orders.Consumers
{
    public class FulfillOrderConsumer : IConsumer<FulfillOrderCommand>
    {
        private static readonly ConcurrentDictionary<int, int> _attempts = new();

        private const int FailUntilAttempt = 3;

        public async Task Consume(ConsumeContext<FulfillOrderCommand> context)
        {

            FulfillOrderCommand command = context.Message;

            var attempt = _attempts.AddOrUpdate(command.OrderId, 1, (_, count) => count + 1);

            if (attempt < FailUntilAttempt)
            {
                Console.WriteLine(
                    $"[FulfillOrderConsumer] Simulating transient failure on attempt {attempt}...");
                throw new InvalidOperationException(
                    $"Transient failure on attempt {attempt} — inventory system unavailable.");
            }

            _attempts.TryRemove(command.OrderId, out _);

            Console.WriteLine($"[FulfillOrderConsumer] Fulfilling order #{context.Message.OrderNumber}");


            await Task.Delay(300);


            Console.WriteLine($"[FulfillOrderConsumer] Order #{context.Message.OrderNumber} fulfilled.");

            await context.Publish(new OrderFulfilledEvent
            {
                OrderId = command.OrderId,
                OrderNumber = command.OrderNumber,
                FulfilledAt = DateTime.UtcNow
            });
        }
    }
}

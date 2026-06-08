using MassTransit;
using MyModularStore.Shared.Events;

namespace MyModularStore.Orders.Consumers
{
    public class OrderConfirmationConsumer : IConsumer<OrderPlacedEvent>
    {
        public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
        {

            Console.WriteLine($"[Confirmation] Sending receipt for order #{context.Message.OrderNumber} " +
                          $"to customer {context.Message.CustomerId}");

            await Task.CompletedTask;
        }
    }
}

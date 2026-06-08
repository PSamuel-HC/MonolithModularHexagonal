using MassTransit;
using MyModularStore.Shared.Events;

namespace MyModularStore.Products.Consumer
{
    public class InventoryUpdateConsumer : IConsumer<OrderPlacedEvent>
    {
        public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
        {
            Console.WriteLine($"[Inventory] Reserving stock for order #{context.Message.OrderNumber}");
            await Task.CompletedTask;
        }
    }
}

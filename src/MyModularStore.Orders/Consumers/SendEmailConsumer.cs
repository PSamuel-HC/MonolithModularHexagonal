using MassTransit;
using MyModularStore.Shared.Events;

namespace MyModularStore.Orders.Consumers
{
    public class SendEmailConsumer : IConsumer<OrderFulfillmentRequestedEvent>
    {
        public async Task Consume(ConsumeContext<OrderFulfillmentRequestedEvent> context)
        {
            var evt = context.Message;
            Console.WriteLine($"[SendEmail] Sending email for order #{evt.OrderNumber}...");

            await Task.Delay(200); // simulate email send

            Console.WriteLine($"[SendEmail] Email sent → publishing EmailSentEvent");

            await context.Publish(new EmailSentEvent
            {
                OrderId = evt.OrderId,
                OrderNumber = evt.OrderNumber
            });
        }
    }
}

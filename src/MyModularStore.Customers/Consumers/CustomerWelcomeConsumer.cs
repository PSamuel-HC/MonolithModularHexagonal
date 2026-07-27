using MassTransit;
using MyModularStore.Shared.Events;

namespace MyModularStore.Customers.Consumers
{
    public class CustomerWelcomeConsumer(ILogger<CustomerWelcomeConsumer> logger)
    : IConsumer<CustomerWelcomeProcessedEvent>
    {
        public Task Consume(ConsumeContext<CustomerWelcomeProcessedEvent> context)
        {
            logger.LogInformation(
                "[Customers] Customer #{CustomerId} ({FullName}) welcome processing completed at {ProcessedAt}",
                context.Message.CustomerId,
                context.Message.FullName,
                context.Message.ProcessedAt);

            return Task.CompletedTask;
        }
    }
}

using MassTransit;
using MyModularStore.Shared.Events;

namespace MyModularStore.ReportWorker.Consumers;

public class CustomerCreatedConsumer(
    ILogger<CustomerCreatedConsumer> logger,
    IPublishEndpoint publishEndpoint) : IConsumer<CustomerCreatedEvent>
{
    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        logger.LogInformation(
            "[ReportWorker] Processing welcome for Customer #{CustomerId} ({FullName})",
            context.Message.CustomerId,
            context.Message.FullName);

        await Task.Delay(2000);

        await publishEndpoint.Publish(new CustomerWelcomeProcessedEvent
        {
            CustomerId = context.Message.CustomerId,
            FullName = context.Message.FullName,
            ProcessedAt = DateTime.UtcNow
        });

        logger.LogInformation(
            "[ReportWorker] Welcome processing completed for Customer #{CustomerId}",
            context.Message.CustomerId);
    }
}

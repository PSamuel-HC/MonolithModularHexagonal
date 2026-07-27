using MassTransit;
using MyModularStore.Shared.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.ReportWorker.Consumers
{
    public class CustomerCreatedConsumer(ILogger<CustomerCreatedConsumer> logger) : IConsumer<CustomerCreatedEvent>
    {
        public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
        {
            var evt = context.Message;

            logger.LogInformation(
                "[ReportWorker] Starting welcome processing for customer #{CustomerId} ({FullName})",
                evt.CustomerId, evt.FullName);

            // Simulate heavy work: generate welcome PDF, sync to CRM, send welcome email, etc.
            await Task.Delay(2000, context.CancellationToken);

            // Publish completion event back — customers-api will pick this up
            await context.Publish(new CustomerWelcomeProcessedEvent
            {
                CustomerId = evt.CustomerId,
                FullName = evt.FullName,
                ProcessedAt = DateTime.UtcNow
            });

            logger.LogInformation(
                "[ReportWorker] Welcome processing done for customer #{CustomerId}",
                evt.CustomerId);
        }
    }
}

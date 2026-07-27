using MassTransit;
using MyModularStore.ReportWorker;
using MyModularStore.ReportWorker.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CustomerCreatedConsumer>();

    var serviceBusConnStr = builder.Configuration.GetConnectionString("ServiceBus");
    if (!string.IsNullOrEmpty(serviceBusConnStr))
    {
        // Cloud: reuse the same Service Bus as orders-api
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(serviceBusConnStr);
            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        // Local: RabbitMQ via docker-compose
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
            {
                h.Username(builder.Configuration["RabbitMq:Username"] ?? "guest");
                h.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
            });

            cfg.ConfigureEndpoints(context);
        });
    }
});

var host = builder.Build();
host.Run();

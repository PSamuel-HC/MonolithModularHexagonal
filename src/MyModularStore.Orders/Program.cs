using FluentValidation;
using MassTransit;
using MyModularStore.Orders;
using MyModularStore.Orders.Consumers;
using MyModularStore.Orders.Infrastructure;
using MyModularStore.Shared.Commands;
using MyModularStore.Shared.Contracts;
using MyModularStore.Shared.Contracts.Http;
using MyModularStore.Shared.ErrorHandling;
using MyModularStore.Shared.ErrorHandling.Handlers;
using MyModularStore.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddOrdersModule(builder.Configuration);

// Orders standalone always runs in microservice mode — use HTTP client for Customers contract
builder.Services.AddHttpClient<ICustomerContract, CustomerHttpClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Customers"]!));

// Exception handler dictionary
builder.Services.AddSingleton(new Dictionary<Type, IErrorHandler>
{
    [typeof(NotFoundException)]               = new NotFoundExceptionHandler(),
    [typeof(RemoteResourceNotFoundException)] = new NotFoundExceptionHandler(),
    [typeof(RemoteServiceException)]          = new ServiceUnavailableExceptionHandler(),
    [typeof(ValidationException)]             = new ValidationExceptionHandler(),
});

builder.Services.AddMassTransit(x =>
{
    ////x.AddEntityFrameworkOutbox<OrderDBContext>(o =>
    ////{
    ////    o.UsePostgres();     // tell MassTransit we're using PostgreSQL
    ////    o.UseBusOutbox();    // use outbox for all Publish and Send calls
    ////});

    x.AddConsumer<FulfillOrderConsumer>();
    x.AddConsumer<OrderConfirmationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-fulfillment", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<FulfillOrderConsumer>(context);
            EndpointConvention.Map<FulfillOrderCommand>(e.InputAddress);
        });

        ////cfg.ReceiveEndpoint("confirmation", e =>
        ////{
        ////    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        ////    e.ConfigureConsumer<OrderConfirmationConsumer>(context);
        ////});

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

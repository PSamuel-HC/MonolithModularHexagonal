using FluentValidation;
using MassTransit;
using MyModularStore.Orders;
using MyModularStore.Orders.Consumers;
using MyModularStore.Orders.Infrastructure;
using MyModularStore.Orders.Infrastructure.Middleware;
using MyModularStore.Orders.Sagas;
using MyModularStore.Shared.Commands;
using MyModularStore.Shared.Contracts;
using MyModularStore.Shared.Contracts.Http;
using MyModularStore.Shared.ErrorHandling;
using MyModularStore.Shared.ErrorHandling.Handlers;
using MyModularStore.Shared.Exceptions;
using Serilog;
using System.Reflection;
using DbUp;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"));


var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")!;

var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();
if (!result.Successful)
{
    Console.Error.WriteLine($"Migration failed: {result.Error}");
    Environment.Exit(1);   // stop the app — schema is not safe to use
}

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

    x.AddSagaStateMachine<OrderSaga, OrderSagaState>()
        .InMemoryRepository();


    ////x.AddEntityFrameworkOutbox<OrderDBContext>(o =>
    ////{
    ////    o.UsePostgres();     // tell MassTransit we're using PostgreSQL
    ////    o.UseBusOutbox();    // use outbox for all Publish and Send calls
    ////});

    x.AddConsumer<FulfillOrderConsumer>();
    x.AddConsumer<SendEmailConsumer>();
    //x.AddConsumer<OrderConfirmationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        //cfg.ReceiveEndpoint("order-fulfillment", e =>
        //{
        //    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
        //    e.ConfigureConsumer<FulfillOrderConsumer>(context);
        //    EndpointConvention.Map<FulfillOrderCommand>(e.InputAddress);
        //});

        ////cfg.ReceiveEndpoint("confirmation", e =>
        ////{
        ////    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        ////    e.ConfigureConsumer<OrderConfirmationConsumer>(context);
        ////});

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

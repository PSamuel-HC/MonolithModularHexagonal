using Azure.Monitor.OpenTelemetry.AspNetCore;
using DbUp;
using MassTransit;
using MyModularStore.Customers;
using MyModularStore.Customers.Consumers;
using MyModularStore.Customers.Endpoints;
using MyModularStore.Customers.GrpcServices;
using MyModularStore.Customers.Operations;
using MyModularStore.Shared.ErrorHandling;
using MyModularStore.Shared.ErrorHandling.Handlers;
using MyModularStore.Shared.Exceptions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

//builder.Services.AddControllers();

var otelBuilder = builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("customers-api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("Npgsql");

        if (builder.Environment.IsDevelopment())
            tracing.AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();

        if (builder.Environment.IsDevelopment())
            metrics.AddConsoleExporter();
    });

var aiConnStr = builder.Configuration.GetConnectionString("ApplicationInsights");
if (!string.IsNullOrEmpty(aiConnStr))
    otelBuilder.UseAzureMonitor(o => o.ConnectionString = aiConnStr);


builder.Services.AddSingleton(new Dictionary<Type, IErrorHandler>
{
    [typeof(NotFoundException)] = new NotFoundExceptionHandler(),
});

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<CustomerWelcomeConsumer>();

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
}

builder.Services.AddCustomersModule();

builder.Services.AddSingleton<OperationStore>(); //could be replaced with a Redis implementation
builder.Services.AddSingleton<OperationQueue>(); //could be replace with a masstransit implementation
builder.Services.AddHostedService<OperationWorker>();
builder.Services.AddHttpClient();


var app = builder.Build();

var connectionString = app.Configuration.GetConnectionString("DefaultConnection")!;
var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();
var result = upgrader.PerformUpgrade();
if (!result.Successful)
{
    Console.Error.WriteLine($"Migration failed: {result.Error}");
    Environment.Exit(1);
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseMiddleware<ExceptionMiddleware>();

app.MapCustomerEndpoints();
app.MapOperationEndpoints();

app.MapGrpcService<CustomerGrpcService>();
if (app.Environment.IsDevelopment())
    app.MapGrpcReflectionService();

//app.UseHttpsRedirection();
//app.MapControllers();
app.Run();

public partial class Program { }

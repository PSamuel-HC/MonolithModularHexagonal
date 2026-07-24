using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using DbUp;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
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
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;
using Serilog;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var kvName = builder.Configuration["KeyVault:Name"];
if (!string.IsNullOrEmpty(kvName))
{
    var kvUri = new Uri($"https://{kvName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(kvUri, new DefaultAzureCredential());
}

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


builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("read-policy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(20);
        opt.PermitLimit = 1;
        opt.SegmentsPerWindow = 5;
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("write-policy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 3;
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please slow down.", ct);

    };

});


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
        ?? "localhost:6379";
});


builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddOrdersModule(builder.Configuration);

// Orders standalone always runs in microservice mode — use HTTP client for Customers contract
builder.Services.AddHttpClient<ICustomerContract, CustomerHttpClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Customers"]!));
    //.AddStandardResilienceHandler();
    //.AddResilienceHandler("customer-pipeline", pipeline =>
    //{
    //    pipeline.AddTimeout(TimeSpan.FromSeconds(120));

    //    pipeline.AddRetry(new HttpRetryStrategyOptions
    //    {
    //        MaxRetryAttempts = 8,
    //        Delay = TimeSpan.FromSeconds(3),
    //        BackoffType = DelayBackoffType.Exponential,
    //        UseJitter = true,
    //        // Only retry on server errors and network failures — never on 4xx
    //        ShouldRetryAfterHeader = true
    //    });

    //    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    //    {
    //        FailureRatio = 0.5,           // 50% failure rate triggers open
    //        SamplingDuration = TimeSpan.FromSeconds(10),
    //        MinimumThroughput = 5,        // need at least 5 calls to evaluate
    //        BreakDuration = TimeSpan.FromSeconds(5)  // stay open 15s before retry
    //    });
    //});

builder.Services.AddResiliencePipeline("database", pipeline =>
{
    pipeline
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            BackoffType = DelayBackoffType.Exponential, //
            UseJitter = true,
            ShouldHandle = new PredicateBuilder()
                .Handle<Npgsql.NpgsqlException>()   // connection/network errors
                .Handle<TimeoutException>()          // query timeout
        })
        .AddTimeout(TimeSpan.FromSeconds(30));       // total pipeline timeout
});


var otelBuilder = builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("orders-api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("Npgsql")
            .AddSource("MassTransit");

        if (builder.Environment.IsDevelopment())
            tracing.AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter("orders-api");

        if (builder.Environment.IsDevelopment())
            metrics.AddConsoleExporter();
    });

var aiConnStr = builder.Configuration.GetConnectionString("ApplicationInsights");
if (!string.IsNullOrEmpty(aiConnStr))
    otelBuilder.UseAzureMonitor(o => o.ConnectionString = aiConnStr);

// Exception handler dictionary
builder.Services.AddSingleton(new Dictionary<Type, IErrorHandler>
{
    [typeof(NotFoundException)]               = new NotFoundExceptionHandler(),
    [typeof(RemoteResourceNotFoundException)] = new NotFoundExceptionHandler(),
    [typeof(RemoteServiceException)]          = new ServiceUnavailableExceptionHandler(),
    [typeof(ValidationException)]             = new ValidationExceptionHandler(),
});

builder.Services.AddSingleton<OrderMetrics>();

if (builder.Configuration.GetValue<bool>("Features:EnableMessaging", true))
{

    builder.Services.AddMassTransit(x =>
    {
        x.AddSagaStateMachine<OrderSaga, OrderSagaState>()
            .InMemoryRepository();

        x.AddConsumer<FulfillOrderConsumer>();
        x.AddConsumer<SendEmailConsumer>();

        var serviceBusConnStr = builder.Configuration.GetConnectionString("ServiceBus");

        if (!string.IsNullOrEmpty(serviceBusConnStr))
        {
            // Cloud: use Azure Service Bus
            x.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(serviceBusConnStr);
                cfg.ConfigureEndpoints(context);
            });
        }
        else
        {
            // Local: use RabbitMQ (docker-compose)
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

var app = builder.Build();

// Liveness — only checks that the process itself is alive (no DB/Redis)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,   // skip all registered checks — just confirm the process responds
    ResponseWriter = WriteJsonResponse
}).DisableRateLimiting();

// Readiness — checks DB + Redis connectivity
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    ResponseWriter = WriteJsonResponse
}).DisableRateLimiting();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

//app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapControllers();

app.Run();


static Task WriteJsonResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    var result = JsonSerializer.Serialize(new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description,
            duration = e.Value.Duration.TotalMilliseconds + "ms"
        })
    });
    return context.Response.WriteAsync(result);
}

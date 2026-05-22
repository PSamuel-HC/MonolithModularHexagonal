using FluentValidation;
using MyModularStore.Orders;
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

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

using System.Reflection;
using DbUp;
using FluentValidation;
using MassTransit;
using MyModularStore.Products;
using MyModularStore.Products.Consumer;
using MyModularStore.Shared.ErrorHandling;
using MyModularStore.Shared.ErrorHandling.Handlers;
using MyModularStore.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
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

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddProductsModule(builder.Configuration);

// Exception handler dictionary
builder.Services.AddSingleton(new Dictionary<Type, IErrorHandler>
{
    [typeof(NotFoundException)] = new NotFoundExceptionHandler(),
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InventoryUpdateConsumer>();

    //if dev or prod
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

//builder.Services.AddApiVersioning(options =>
//{
//    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
//}).AddMvc();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
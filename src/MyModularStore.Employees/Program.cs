
using MyModularStore.Employees;
using MyModularStore.Shared.ErrorHandling;
using MyModularStore.Shared.ErrorHandling.Handlers;
using MyModularStore.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddEmployeesModule(builder.Configuration);

// Exception handler dictionary
builder.Services.AddSingleton(new Dictionary<Type, IErrorHandler>
{
    [typeof(NotFoundException)] = new NotFoundExceptionHandler(),
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
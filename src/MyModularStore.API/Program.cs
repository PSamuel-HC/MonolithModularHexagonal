using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyModularStore.Customers;
using MyModularStore.Customers.Application;
using MyModularStore.Employees;
using MyModularStore.Employees.Application;
using MyModularStore.Orders;
using MyModularStore.Products;
using MyModularStore.Products.Application;
using MyModularStore.Shared.Contracts;
using MyModularStore.Shared.Contracts.Http;
using FluentValidation;
using MyModularStore.Shared.ErrorHandling;
using MyModularStore.Shared.ErrorHandling.Handlers;
using MyModularStore.Shared.Exceptions;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Module DI registrations
builder.Services.AddProductsModule(builder.Configuration);
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddEmployeesModule(builder.Configuration);
builder.Services.AddCustomersModule(builder.Configuration);

var deploymentMode = builder.Configuration["Deployment:Mode"];

if (deploymentMode == "Monolith")
{
    builder.Services.AddScoped<ICustomerContract>(
        sp => sp.GetRequiredService<CustomerService>());
    builder.Services.AddScoped<IProductContract>(
        sp => sp.GetRequiredService<ProductService>());
    builder.Services.AddScoped<IEmployeeContract>(
        sp => sp.GetRequiredService<EmployeeService>());
}
else
{
    builder.Services.AddHttpClient<ICustomerContract, CustomerHttpClient>(client =>
        client.BaseAddress = new Uri(builder.Configuration["Services:Customers"]!));
    builder.Services.AddHttpClient<IProductContract, ProductHttpClient>(client =>
        client.BaseAddress = new Uri(builder.Configuration["Services:Products"]!));
}


// Exception handler dictionary — adapts the Executor/Dictionary pattern to middleware
builder.Services.AddSingleton(new Dictionary<Type, IErrorHandler>
{
    [typeof(NotFoundException)]                 = new NotFoundExceptionHandler(),
    [typeof(RemoteResourceNotFoundException)]   = new NotFoundExceptionHandler(),
    [typeof(RemoteServiceException)]            = new ServiceUnavailableExceptionHandler(),
    [typeof(ValidationException)]               = new ValidationExceptionHandler(),
});

//Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
}).AddMvc();

//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

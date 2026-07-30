using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using MyModularStore.Customers.Application.DTOs;
using MyModularStore.Customers.Application.Ports;
using MyModularStore.Customers.Features.GetCustomerById;

namespace MyModularStore.Customers.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void MapCustomerEndpoints(this WebApplication app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/customers")
                .WithTags("Customers");

            group.MapGet("/", GetCustomers);
            group.MapGet("/{id}", GetCustomer);
            group.MapPost("/", CreateCustomer);
            group.MapPut("/{id}", UpdateCustomer);
            group.MapDelete("/{id}", DeleteCustomer);
            group.MapGet("/{id}/exists", CustomerExists);
        }

        static async Task<Ok<IEnumerable<CustomerDto>>> GetCustomers(ICustomerModule custumerModule)
        {
            return TypedResults.Ok(await custumerModule.GetCustomersAsync());
        }

        static async Task<Results<Ok<CustomerDto>, NotFound>> GetCustomer(
        int id, ISender sender)
        {
            var customer = await sender.Send(new GetCustomerByIdQuery(id));
            return customer is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(customer);
        }

        static async Task<Created<CustomerDto>> CreateCustomer(
        CustomerCreateDto dto, ICustomerModule module)
        {
            var result = await module.CreateCustomerAsync(dto);
            return TypedResults.Created($"/api/customers/{result.Id}", result);
        }

        static async Task<Results<NoContent, NotFound>> UpdateCustomer(
        int id, CustomerUpdateDto dto, ICustomerModule module)
        {
            var updated = await module.UpdateCustomerAsync(id, dto);
            return updated
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        }

        static async Task<Results<NoContent, NotFound>> DeleteCustomer(
        int id, ICustomerModule module)
        {
            var deleted = await module.DeleteCustomerAsync(id);
            return deleted
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        }

        static async Task<Ok<bool>> CustomerExists(
        int id, ICustomerModule module)
        {
            return TypedResults.Ok(await module.ExistsAsync(id));
        }
    }
}

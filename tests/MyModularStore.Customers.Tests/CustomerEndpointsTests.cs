using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyModularStore.Customers.Application.DTOs;

namespace MyModularStore.Customers.Tests;

public class CustomerEndpointsTests(CustomersApiFactory factory)
    : IClassFixture<CustomersApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetCustomers_ReturnsOkWithEmptyList()
    {
        var response = await _client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
        customers.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreatedWithCustomer()
    {
        var dto = new CustomerCreateDto
        {
            FullName = "Alice Johnson",
            Email = "alice@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CustomerDto>();
        created!.Id.Should().BeGreaterThan(0);
        created.FullName.Should().Be("Alice Johnson");
        created.Email.Should().Be("alice@example.com");
    }

    [Fact]
    public async Task GetCustomer_WhenExists_ReturnsOkWithCustomer()
    {
        var dto = new CustomerCreateDto { FullName = "Bob Smith", Email = "bob@example.com" };
        var createResponse = await _client.PostAsJsonAsync("/api/customers", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var response = await _client.GetAsync($"/api/customers/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer!.Email.Should().Be("bob@example.com");
        customer.FullName.Should().Be("Bob Smith");
    }

    [Fact]
    public async Task GetCustomer_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/customers/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCustomer_WhenExists_ReturnsNoContent()
    {
        var dto = new CustomerCreateDto { FullName = "To Delete", Email = "delete@example.com" };
        var createResponse = await _client.PostAsJsonAsync("/api/customers", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/customers/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync($"/api/customers/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

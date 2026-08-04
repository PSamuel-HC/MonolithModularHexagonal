using FluentAssertions;
using MyModularStore.Customers.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace MyModularStore.Customers.Tests
{
    public class CustomerEndpointsTests(CustomersApiFactory factory) : IClassFixture<CustomersApiFactory>
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
        public async Task GetCustomer_WhenNotFound_Returns404()
        {
            var response = await _client.GetAsync("/api/customers/99999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

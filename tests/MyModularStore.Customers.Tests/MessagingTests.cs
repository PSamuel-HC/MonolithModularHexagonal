using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MyModularStore.Customers.Application.DTOs;
using MyModularStore.Shared.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace MyModularStore.Customers.Tests
{
    public class MessagingTests(CustomersApiFactory factory) : IClassFixture<CustomersApiFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();


        [Fact]
        public async Task CreateCustomer_PublishesCustomerCreatedEvent()
        {
            var harness = factory.Services.GetRequiredService<ITestHarness>();

            var dto = new CustomerCreateDto
            {
                FullName = "Event Test User",
                Email = "event@example.com"
            };

            var response = await _client.PostAsJsonAsync("/api/customers", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            (await harness.Published.Any<CustomerCreatedEvent>()).Should().BeTrue();
        }

        [Fact]
        public async Task CreateCustomer_PublishedEvent_HasCorrectData()
        {
            var harness = factory.Services.GetRequiredService<ITestHarness>();

            var dto = new CustomerCreateDto
            {
                FullName = "Maria Silva",
                Email = "maria@example.com"
            };

            await _client.PostAsJsonAsync("/api/customers", dto);

            var published = harness.Published.Select<CustomerCreatedEvent>().LastOrDefault();
            published.Should().NotBeNull();
            published!.Context.Message.FullName.Should().Be("Maria Silva");
            published.Context.Message.Email.Should().Be("maria@example.com");
            published.Context.Message.CustomerId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CustomerWelcomeConsumer_WhenReceivesEvent_DoesNotFault()
        {
            var harness = factory.Services.GetRequiredService<ITestHarness>();

            await harness.Bus.Publish(new CustomerWelcomeProcessedEvent
            {
                CustomerId = 42,
                FullName = "Consumer Test User",
                ProcessedAt = DateTime.UtcNow
            });

            // Any<T>() is a snapshot check — give the in-memory bus time to dispatch to the consumer
            await Task.Delay(500);

            (await harness.Consumed.Any<CustomerWelcomeProcessedEvent>()).Should().BeTrue();
            (await harness.Published.Any<Fault<CustomerWelcomeProcessedEvent>>()).Should().BeFalse();
        }

    }
}

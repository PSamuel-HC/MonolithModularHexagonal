using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyModularStore.Customers.Consumers;
using Testcontainers.PostgreSql;

namespace MyModularStore.Customers.Tests;

public class CustomersApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithEnvironment("POSTGRES_INITDB_ARGS", "--nosync")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var harness = Services.GetRequiredService<ITestHarness>();
        await harness.Start();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString()
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<CustomerWelcomeConsumer>();
            });
        });
    }

    public new async Task DisposeAsync()
    {
        await Services.GetRequiredService<ITestHarness>().Stop();
        await _postgres.DisposeAsync();
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.PostgreSql;

namespace MyModularStore.Customers.Tests
{
    public class CustomersApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgre = new PostgreSqlBuilder("postgres:16-alpine")
            .WithEnvironment("POSTGRES_INITDB_ARGS", "--nosync")
            .Build();


        public async Task InitializeAsync()
        {
            await _postgre.StartAsync();
        }


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _postgre.GetConnectionString()
                });
            });
        }


        async Task IAsyncLifetime.DisposeAsync()
        {
            await _postgre.DisposeAsync();
        }
    }
}

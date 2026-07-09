using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyModularStore.Orders.Application.Ports;

namespace MyModularStore.Orders.Infrastructure.Services
{
    public class OrderStatsService(
        IServiceScopeFactory scopeFactory,
        ILogger<OrderStatsService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("OrderStatsService started");

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            await RunStatsAsync(stoppingToken);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunStatsAsync(stoppingToken);
            }
        }

        private async Task RunStatsAsync(CancellationToken ct)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            var orders = await repository.GetAllAsync(ct);
            var list = orders.ToList();

            var byStatus = list
                .GroupBy(o => o.Status.ToString())
                .Select(g => $"{g.Key}: {g.Count()}")
                .ToList();

            logger.LogInformation(
                "Order stats — Total: {Total} | {ByStatus}",
                list.Count,
                string.Join(" | ", byStatus));
        }
    }
}
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyModularStore.Orders.Infrastructure.Health
{
    public class RedisHealthCheck(IDistributedCache cache) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await cache.SetStringAsync("health:ping", "pong",
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                    }, cancellationToken);

                var value = await cache.GetStringAsync("health:ping", cancellationToken);

                return value == "pong"
                    ? HealthCheckResult.Healthy("Redis is reachable")
                    : HealthCheckResult.Degraded("Redis responded but value was unexpected");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis is unreachable", ex);
            }
        }
    }
}

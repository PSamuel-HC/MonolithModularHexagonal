using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MyModularStore.Customers.Operations
{
    public class OperationWorker(
        OperationQueue queue,
        OperationStore store,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OperationWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var item in queue.Reader.ReadAllAsync(stoppingToken))
            {
                await ProcessAsync(item, stoppingToken);
            }
        }

        private async Task ProcessAsync(OperationWorkItem item, CancellationToken ct)
        {
            store.Update(item.OperationId, OperationStatus.Processing);

            // Simulate heavy work (replace with real logic)
            await Task.Delay(TimeSpan.FromSeconds(8), ct);

            var result = new
            {
                CustomerId = item.CustomerId,
                ReportUrl = $"/reports/customer-{item.CustomerId}.pdf",
                GeneratedAt = DateTime.UtcNow
            };

            store.Update(item.OperationId, OperationStatus.Completed, result: result);

            //send notifications
            if (!string.IsNullOrEmpty(item.WebhookUrl))
                await SendWebhookAsync(item.WebhookUrl, item.OperationId, "Completed", result, ct);
        }

        private async Task SendWebhookAsync(
        string url, string operationId, string status, object? result, CancellationToken ct)
        {
            try
            {
                var payload = new { operationId, status, result };
                var json = JsonSerializer.Serialize(payload);
                var secret = configuration["Webhooks:Secret"] ?? "dev-secret";
                var signature = ComputeSignature(json, secret);

                using var client = httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("X-Signature-SHA256", signature);

                await client.SendAsync(request, ct);
                logger.LogInformation("Webhook delivered to {Url} for operation {OperationId}", url, operationId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Webhook delivery failed for operation {OperationId}", operationId);
            }
        }

        private static string ComputeSignature(string payload, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            var data = Encoding.UTF8.GetBytes(payload);
            using var hmac = new HMACSHA256(key);
            return Convert.ToHexString(hmac.ComputeHash(data)).ToLowerInvariant();
        }
    }
}

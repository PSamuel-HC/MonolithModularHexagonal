using Microsoft.Azure.Cosmos;
using MyModularStore.Products.Domain;

namespace MyModularStore.Products.Infrastructure
{
    public class SubscriptionPolicyRepository(CosmosClient cosmosClient)
    {
        private readonly Container _container =
            cosmosClient.GetContainer("store-db", "api-keys");

        public async Task<SubscriptionPolicy?> GetByKeyAsync(string subscriptionKey, CancellationToken ct = default)
        {
            try
            {
                var response = await _container.ReadItemAsync<SubscriptionPolicy>(
                    subscriptionKey,
                    new PartitionKey(subscriptionKey),
                    cancellationToken: ct);

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}

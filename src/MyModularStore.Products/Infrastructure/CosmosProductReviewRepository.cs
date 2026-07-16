using Microsoft.Azure.Cosmos;
using MyModularStore.Products.Domain;

namespace MyModularStore.Products.Infrastructure
{
    public class CosmosProductReviewRepository(CosmosClient cosmosClient)
    {
        private readonly Container _container =
            cosmosClient.GetContainer("store-db", "product-reviews");

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(
            string productId, CancellationToken ct = default)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.productId = @productId")
                .WithParameter("@productId", productId);

            var iterator = _container.GetItemQueryIterator<ProductReview>(
                query,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(productId)
                });

            var results = new List<ProductReview>();
            while (iterator.HasMoreResults)
            {
                var page = await iterator.ReadNextAsync(ct);
                results.AddRange(page);
            }
            return results;
        }

        public async Task AddAsync(ProductReview review, CancellationToken ct = default)
        {
            await _container.CreateItemAsync(
                review,
                new PartitionKey(review.ProductId),
                cancellationToken: ct);
        }
    }
}

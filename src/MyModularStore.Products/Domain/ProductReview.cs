using Newtonsoft.Json;

namespace MyModularStore.Products.Domain
{
    public class ProductReview
    {
        [JsonProperty("id")]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [JsonProperty("productId")]
        public string ProductId { get; init; } = string.Empty;

        [JsonProperty("customerId")]
        public int CustomerId { get; init; }

        [JsonProperty("rating")]
        public int Rating { get; init; }

        [JsonProperty("comment")]
        public string Comment { get; init; } = string.Empty;

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}

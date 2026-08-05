using Newtonsoft.Json;

namespace MyModularStore.Products.Domain
{
    public class SubscriptionPolicy
    {
        [JsonProperty("id")]
        public string Id { get; init; } = string.Empty;

        [JsonProperty("allowedEndpoints")]
        public List<string> AllowedEndpoints { get; init; } = [];

        [JsonProperty("tier")]
        public string Tier { get; init; } = string.Empty;

        [JsonProperty("isActive")]
        public bool IsActive { get; init; }
    }
}

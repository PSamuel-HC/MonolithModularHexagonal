namespace MyModularStore.Products.Domain
{
    public class ProductEmbedding
    {
        public int ProductId { get; set; }
        public string Description { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = [];
    }
}

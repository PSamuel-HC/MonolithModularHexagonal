namespace MyModularStore.Products.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int WarrantyMonths { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

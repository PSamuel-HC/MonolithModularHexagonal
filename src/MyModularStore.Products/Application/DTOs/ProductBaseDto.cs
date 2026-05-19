namespace MyModularStore.Products.Application.DTOs
{
    public abstract class ProductBaseDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public int WarrantyMonths { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

namespace MyModularStore.Products.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public int WarrantyMonths { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

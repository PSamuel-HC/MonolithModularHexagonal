namespace MyModularStore.Orders.Application.DTOs
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string EstimatedDelivery { get; set; } = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddTHH:00:00Z");
    }
}

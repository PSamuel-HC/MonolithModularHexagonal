using MyModularStore.Orders.Domain.Enums;

namespace MyModularStore.Orders.Application.DTOs
{
    public class OrderBaseDto
    {
        public string? OrderNumber { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal? TotalAmount { get; set; }
        public OrderStatus? Status { get; set; }
        public string? ShippingAddress { get; set; }
    }
}

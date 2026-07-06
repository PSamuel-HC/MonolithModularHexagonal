using MyModularStore.Orders.Domain.Enums;

namespace MyModularStore.Orders.Application.DTOs
{
    public class OrderWithCustomerReadDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}

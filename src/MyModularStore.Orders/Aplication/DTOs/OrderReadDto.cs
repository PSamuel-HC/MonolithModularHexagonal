using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Orders.Aplication.DTOs
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string EstimatedDelivery { get; set; } = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddTHH:00:00Z");
    }
}

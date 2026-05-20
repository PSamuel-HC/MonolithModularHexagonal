using MyModularStore.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Orders.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; } // The Primary Key
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string ShippingAddress { get; set; } = string.Empty;
    }
}

using MyModularStore.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Orders.Aplication.DTOs
{
    public class OrderCreateDto
    {
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string ShippingAddress { get; set; }
    }
}

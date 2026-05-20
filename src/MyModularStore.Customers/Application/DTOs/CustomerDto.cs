using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Customers.Application.DTOs
{
    public class CustomerDto : CustomerBaseDto
    {
        public int Id { get; set; }

        public int PointsBalance { get; set; }

        public DateTime? LastPurchaseDate { get; set; }
    }
}

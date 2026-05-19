using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Shared.Contracts
{
    public class ProductInfo
    {
        public int Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}

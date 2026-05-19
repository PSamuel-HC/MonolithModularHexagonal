using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Products.Application.DTOs
{
    public class CreateProductDto : ProductBaseDto
    {
        public string SKU { get; set; } = string.Empty;
    }
}

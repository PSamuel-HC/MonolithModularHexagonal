using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Customers.Application.DTOs
{
    public  class CustomerBaseDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
    }
}

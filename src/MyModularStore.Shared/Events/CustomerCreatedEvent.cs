using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Shared.Events
{
    public class CustomerCreatedEvent
    {
        public int CustomerId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Shared.Events
{
    public class CustomerWelcomeProcessedEvent
    {
        public int CustomerId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public DateTime ProcessedAt { get; init; }
    }
}

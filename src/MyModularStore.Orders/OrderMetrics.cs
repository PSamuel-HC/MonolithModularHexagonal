using System.Diagnostics.Metrics;

namespace MyModularStore.Orders
{
    public class OrderMetrics
    {
        private readonly Counter<int> _ordersCreated;
        private readonly Histogram<double> _processingTimeMs;

        public OrderMetrics(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create("orders-api");

            _ordersCreated = meter.CreateCounter<int>(
                "orders.created",
                unit: "orders",
                description: "Total number of orders created");

            _processingTimeMs = meter.CreateHistogram<double>(
                "orders.processing_time",
                unit: "ms",
                description: "Time to process a CreateOrder request");
        }

        public void OrderCreated() => _ordersCreated.Add(1);

        public void RecordProcessingTime(double milliseconds) =>
                _processingTimeMs.Record(milliseconds);
    }
}

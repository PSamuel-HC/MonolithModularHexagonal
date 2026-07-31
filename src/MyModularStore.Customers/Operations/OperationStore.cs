using System.Collections.Concurrent;

namespace MyModularStore.Customers.Operations
{
    public class OperationStore
    {
        private readonly ConcurrentDictionary<string, OperationRecord> _store = new();

        public OperationRecord Start()
        {
            var op = new OperationRecord(
                Id: Guid.NewGuid().ToString("N"),
                Status: OperationStatus.Pending,
                Result: null,
                Error: null,
                CreatedAt: DateTime.UtcNow,
                CompletedAt: null);

            _store[op.Id] = op;
            return op;
        }


        public void Update(string id, OperationStatus status, object? result = null, string? error = null)
        {
            if (_store.TryGetValue(id, out var op))
                _store[id] = op with
                {
                    Status = status,
                    Result = result,
                    Error = error,
                    CompletedAt = status is OperationStatus.Completed or OperationStatus.Failed
                        ? DateTime.UtcNow : null
                };
        }

        public OperationRecord? Get(string id) =>
            _store.TryGetValue(id, out var op) ? op : null;

    }
}

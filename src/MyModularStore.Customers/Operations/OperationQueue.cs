using System.Threading.Channels;

namespace MyModularStore.Customers.Operations
{
    public class OperationQueue
    {
        private readonly Channel<OperationWorkItem> _channel =
            Channel.CreateUnbounded<OperationWorkItem>();

        public ChannelReader<OperationWorkItem> Reader => _channel.Reader;

        public void Enqueue(OperationWorkItem item) =>
            _channel.Writer.TryWrite(item);

    }
}

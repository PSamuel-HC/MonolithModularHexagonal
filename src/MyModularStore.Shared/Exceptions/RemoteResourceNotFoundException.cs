namespace MyModularStore.Shared.Exceptions
{
    public class RemoteResourceNotFoundException : AppException
    {
        public override int StatusCode => 404;

        public RemoteResourceNotFoundException(string message) : base(message) { }
    }
}

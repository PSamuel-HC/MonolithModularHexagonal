namespace MyModularStore.Shared.Exceptions;

/// <summary>
/// Thrown by HTTP adapters when the remote module returns an unexpected error.
/// Keeps the calling module isolated from the source module's exception types.
/// </summary>
public class RemoteServiceException : AppException
{
    public override int StatusCode => 503;
    public RemoteServiceException(string message) : base(message) { }
}

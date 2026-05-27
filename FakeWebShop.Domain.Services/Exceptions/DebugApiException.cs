namespace FakeWebShop.Domain.Services.Exceptions;

public class DebugApiException : Exception
{
    public int StatusCode { get; }
    public string? ErrorCode { get; }

    public DebugApiException(int statusCode, string message, string? errorCode = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
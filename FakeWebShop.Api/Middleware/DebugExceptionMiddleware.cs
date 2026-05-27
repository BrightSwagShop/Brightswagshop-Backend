using System.Text.Json;
using FakeWebShop.Domain.Services.Exceptions;

namespace FakeWebShop.Api.Middleware;

public class DebugExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public DebugExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DebugApiException dex)
        {
            context.Response.StatusCode = dex.StatusCode;
            context.Response.ContentType = "application/json";
            var payload = new { error = dex.Message, code = dex.ErrorCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var payload = new { error = "Internal Server Error", detail = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
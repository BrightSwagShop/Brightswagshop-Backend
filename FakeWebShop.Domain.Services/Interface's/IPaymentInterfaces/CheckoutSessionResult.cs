using System;

namespace FakeWebShop.Domain.Services.Interface_s;

public class CheckoutSessionResult
{
    public string SessionId { get; set; } = string.Empty;
    public string SessionUrl { get; set; } = string.Empty;
}

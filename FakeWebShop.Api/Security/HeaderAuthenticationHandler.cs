using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FakeWebShop.Api.Security;

public class HeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string UserIdHeader = "X-User-Id";
    public const string RoleHeader = "X-User-Role";

    public HeaderAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserIdHeader, out var userIdValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userId = userIdValues.ToString();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Task.FromResult(AuthenticateResult.Fail("X-User-Id header is verplicht."));
        }

        var role = Request.Headers.TryGetValue(RoleHeader, out var roleValues)
            ? roleValues.ToString().Trim()
            : string.Empty;

        if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            role = "Admin";
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("user_id", userId)
        };

        if (!string.IsNullOrWhiteSpace(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, HeaderAuthDefaults.Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, HeaderAuthDefaults.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

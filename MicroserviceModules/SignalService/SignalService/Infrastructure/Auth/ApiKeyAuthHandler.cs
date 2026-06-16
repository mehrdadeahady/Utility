using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SignalService.Infrastructure.Auth
{
    public class ApiKeyAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ApiKeyAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Api-Key", out var providedKey))
                return Task.FromResult(AuthenticateResult.Fail("Missing API key"));

            var config = Context.RequestServices.GetRequiredService<IConfiguration>();
            var validKey = config["ApiKeys:Default"];

            if (providedKey != validKey)
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));

            var identity = new ClaimsIdentity("ApiKey");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

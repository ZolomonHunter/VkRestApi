using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace VkRestApi.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        // Basic authorization implementation
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            const string FailureMessage = "Please pass base64-encoded login and password in header's Authorization argument " +
                    "(Default is admin:admin -> \"Basic YWRtaW46YWRtaW4=\")";
            
            // Getting credentials from header
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail(FailureMessage);
            string? encodedCredentials = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).Parameter;
            if (encodedCredentials == null)
                return AuthenticateResult.Fail("Authorization field is empty");
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials)).Split(":");
            string username = credentials[0];
            string password = credentials[1];

            // Hardcoded admin:admin credentials for access
            if (username == "admin" && password == "admin")
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            else
                return AuthenticateResult.Fail(FailureMessage);

            return AuthenticateResult.Fail("Error happened");
        }
    }
}

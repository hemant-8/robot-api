using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using robot_controller_api.Persistence;
using robot_controller_api.Models;

namespace robot_controller_api.Authentication;

public class BasicAuthenticationHandler 
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // ✅ Allow Swagger + root (no auth required)
        if (Request.Path.StartsWithSegments("/swagger") || Request.Path == "/")
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // ✅ Check Authorization header
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        }

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            var encoded = authHeader.Replace("Basic ", "");
            var bytes = Convert.FromBase64String(encoded);
            var credentials = Encoding.UTF8.GetString(bytes);

            var parts = credentials.Split(":");
            if (parts.Length != 2)
                return Task.FromResult(AuthenticateResult.Fail("Invalid Header"));

            var email = parts[0];
            var password = parts[1];

            var userData = Context.RequestServices.GetRequiredService<UserDataAccess>();
            var user = userData.GetByEmail(email);

            if (user == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("User not found"));
            }

            // 🔥 SECURE PASSWORD CHECK (HASHED)
            var hasher = new PasswordHasher<UserModel>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Task.FromResult(AuthenticateResult.Fail("Wrong password"));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Header"));
        }
    }
}
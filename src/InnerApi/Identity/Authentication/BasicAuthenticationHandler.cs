using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SFA.DAS.Apprenticeships.InnerApi.Identity.Authentication;

[ExcludeFromCodeCoverage]
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

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var claims = new[] {
			new Claim(ClaimTypes.NameIdentifier, "OuterApi"),
			new Claim(ClaimTypes.Name, "OuterApi"),
			new Claim(ClaimTypes.Role, "OuterApi")
		};
		var identity = new ClaimsIdentity(claims, Scheme.Name);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, Scheme.Name);

		return await Task.FromResult(AuthenticateResult.Success(ticket));
	}
}

using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.InnerApi.Identity.Authorization;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Identity.Authorization
{
    public class WhenInvokingBearerTokenMiddleware
    {
        private BearerTokenMiddleware _middleware;
        private Mock<IConfiguration> _mockConfiguration;
        private HttpContext _httpContext;
        private Mock<ILogger<BearerTokenMiddleware>> _mockLogger;
        private const string ValidSigningKey = "abcdefghijklmnopqrstuv123456789==";

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<BearerTokenMiddleware>>();
            _middleware = new BearerTokenMiddleware(Next, _mockConfiguration.Object, _mockLogger.Object);
            _httpContext = new DefaultHttpContext();
        }

        [Test]
        public async Task WhenValidTokenWithUkprnClaim_ShouldSetUkprnInContextItems()
        {
            // Arrange
            var claims = new List<Claim> { new ("http://schemas.portal.com/ukprn", "12345"), new(ClaimTypes.Name, "ProviderUserName") };
            var token = TestAuthorizationHelper.CreateValidToken(ValidSigningKey, claims);
            _httpContext.Request.Headers["Authorization"] = $"{token}";
            SetUserBearerTokenSigningKeyConfig(ValidSigningKey);
            SetDisableAccountAuthorisationConfig(false);

            // Act
            await _middleware.Invoke(_httpContext);
            
            // Assert
            _httpContext.Items.Should().ContainKey("Ukprn");
            _httpContext.Items["Ukprn"].Should().Be("12345");
        }

        [Test]
        public async Task WhenValidTokenWithEmployerAccountIdClaim_ShouldSetEmployerAccountIdInContextItems()
        {
            // Arrange
            var claims = new List<Claim> { new ("http://das/employer/identity/claims/account", "98765"), new("http://das/employer/identity/claims/id", "EmployerUserName") };
            var token = TestAuthorizationHelper.CreateValidToken(ValidSigningKey, claims);
            _httpContext.Request.Headers["Authorization"] = $"{token}";
            SetUserBearerTokenSigningKeyConfig(ValidSigningKey);
            SetDisableAccountAuthorisationConfig(false);

            // Act
            await _middleware.Invoke(_httpContext);

            // Assert
            _httpContext.Items.Should().ContainKey("EmployerAccountId");
            _httpContext.Items["EmployerAccountId"].Should().Be("98765");
        }

        [Test]
        public async Task WhenInvalidToken_ReturnUnauthorizedStatus()
        {
            // Arrange
            SetDisableAccountAuthorisationConfig(false);
            SetUserBearerTokenSigningKeyConfig(ValidSigningKey);

            // Act
            var response = await SendGetRequestToTestClient("invalid_token");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task WhenTokenMissing_ReturnUnauthorizedStatus()
        {
            // Arrange & Act
            var response = await SendGetRequestToTestClient();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private static async Task<HttpResponseMessage> SendGetRequestToTestClient(string bearerToken = null)
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .Configure(app => { app.UseMiddleware<BearerTokenMiddleware>(); });
                })
                .StartAsync();

            var client = host.GetTestClient();
            if (bearerToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            var response = await host.GetTestClient().GetAsync("/");
            return response;
        }

        private Task Next(HttpContext context)
        {
            return Task.CompletedTask;
        }

        private void SetDisableAccountAuthorisationConfig(bool disableAccountAuthorisation)
        {
            _mockConfiguration.Setup(x => x["DisableAccountAuthorisation"]).Returns(disableAccountAuthorisation.ToString());
        }
        
        private void SetUserBearerTokenSigningKeyConfig(string signingKey)
        {
            _mockConfiguration.Setup(x => x["UserBearerTokenSigningKey"]).Returns(signingKey);
        }
    }
}

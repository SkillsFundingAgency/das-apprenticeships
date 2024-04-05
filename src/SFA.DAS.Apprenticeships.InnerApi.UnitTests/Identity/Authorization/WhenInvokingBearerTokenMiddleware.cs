using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Identity.Authorization
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
        public async Task WhenDisableAccountAuthorizationTrue_ShouldNotThrowException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DisableAccountAuthorization"]).Returns("true");

            // Act & Assert
            Func<Task> action = async () => await _middleware.Invoke(_httpContext);
            await action.Should().NotThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task WhenTokenMissing_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DisableAccountAuthorization"]).Returns("false");

            // Act & Assert
            Func<Task> action = async () => await _middleware.Invoke(_httpContext);
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task WhenValidTokenWithUkprnClaim_ShouldSetUkprnInContextItems()
        {
            // Arrange
            var claims = new List<Claim> { new ("http://schemas.portal.com/ukprn", "12345") };
            var token = CreateValidToken(ValidSigningKey, claims);
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
            var claims = new List<Claim> { new ("http://das/employer/identity/claims/account", "98765") };
            var token = CreateValidToken(ValidSigningKey, claims);
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
        public async Task WhenInvalidToken_ThrowSecurityTokenArgumentException()
        {
            // Arrange
            _httpContext.Request.Headers["Authorization"] = "invalid_token";
            SetDisableAccountAuthorisationConfig(false);
            SetUserBearerTokenSigningKeyConfig(ValidSigningKey);

            // Act
            var action = async () => await _middleware.Invoke(_httpContext);

            // Assert
            await action.Should().ThrowAsync<SecurityTokenArgumentException>();
        }

        [Test]
        public async Task WhenNoTokenProvided_ThrowUnauthorizedAccessException()
        {
            // Act
            var action = async () => await _middleware.Invoke(_httpContext);

            // Assert
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task WhenNoSigningKey_ThrowArgumentNullException(string? signingKey)
        {
            // Arrange
            var claims = new List<Claim> { new ("http://das/employer/identity/claims/account", "98765") };
            var token = CreateValidToken(ValidSigningKey, claims);
            _httpContext.Request.Headers["Authorization"] = $"{token}";
            SetUserBearerTokenSigningKeyConfig(ValidSigningKey);
            SetDisableAccountAuthorisationConfig(false);
            SetUserBearerTokenSigningKeyConfig("");

            // Act
            var action = async () => await _middleware.Invoke(_httpContext);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
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
        private static string CreateValidToken(string signingKey, List<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Identity.Authorization
{
    public class BearerTokenMiddlewareTests
    {
        private BearerTokenMiddleware _middleware;
        private Mock<IConfiguration> _mockConfiguration;
        private HttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _middleware = new BearerTokenMiddleware(Next, _mockConfiguration.Object);
            _httpContext = new DefaultHttpContext();
        }

        private Task Next(HttpContext context)
        {
            return Task.CompletedTask;
        }

        [Test]
        public async Task Invoke_WhenDisableAccountAuthorizationTrue_ShouldNotThrowException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DisableAccountAuthorization"]).Returns("true");

            // Act & Assert
            Func<Task> action = async () => await _middleware.Invoke(_httpContext);
            await action.Should().NotThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task Invoke_WhenTokenMissing_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DisableAccountAuthorization"]).Returns("false");

            // Act & Assert
            Func<Task> action = async () => await _middleware.Invoke(_httpContext);
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}

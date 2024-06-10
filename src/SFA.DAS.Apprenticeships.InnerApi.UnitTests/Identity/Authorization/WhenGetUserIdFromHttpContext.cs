using Microsoft.AspNetCore.Http;
using Moq;
using SFA.DAS.Apprenticeships.InnerApi.Identity.Authorization;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Identity.Authorization;

public class WhenGetUserIdFromHttpContext
{
    [Test]
    public void WhenUserIdExists_ReturnsUserId()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.Items).Returns(new Dictionary<object, object?> { { "UserId", "testUserId" } });

        // Act
        var result = mockHttpContext.Object.GetUserId();

        // Assert
        Assert.That(result.Equals("testUserId"));
    }

    [Test]
    public void WhenUserIdDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.Items).Returns(new Dictionary<object, object?>());

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => mockHttpContext.Object.GetUserId());
        Assert.That(exception.Message.Equals("User Id not found in HttpContext"));
    }

    [Test]
    public void WhenUserIdIsEmptyString_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.Items).Returns(new Dictionary<object, object?> { { "UserId", "" } });

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => mockHttpContext.Object.GetUserId());
        Assert.That(exception.Message.Equals("User Id Key exists in HttpContext, but the value is empty"));
    }
}

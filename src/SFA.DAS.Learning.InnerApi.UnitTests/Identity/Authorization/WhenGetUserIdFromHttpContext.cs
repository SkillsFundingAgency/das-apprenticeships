using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SFA.DAS.Learning.InnerApi.Identity.Authorization;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Identity.Authorization;

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
       result.Should().Be("testUserId");
    }

    [Test]
    public void WhenUserIdDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.Items).Returns(new Dictionary<object, object?>());

        // Act 
        var action = () => mockHttpContext.Object.GetUserId();

        // Assert
        action.Should().Throw<UnauthorizedAccessException>().WithMessage("User Id not found in HttpContext");
    }

    [Test]
    public void WhenUserIdIsEmptyString_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.Items).Returns(new Dictionary<object, object?> { { "UserId", "" } });

        // Act 
        var action = () => mockHttpContext.Object.GetUserId();

        // Assert
        action.Should().Throw<UnauthorizedAccessException>().WithMessage("User Id Key exists in HttpContext, but the value is empty");
    }
}

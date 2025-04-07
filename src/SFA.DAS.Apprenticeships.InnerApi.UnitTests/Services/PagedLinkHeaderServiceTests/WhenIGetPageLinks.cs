using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using SFA.DAS.Apprenticeships.InnerApi.Services;
using SFA.DAS.Apprenticeships.Queries;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByDates;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Services.PagedLinkHeaderServiceTests;

public class WhenIGetPageLinks
{
    private const string Schema = "https";
    private const string Domain = "mydomain.test";
    private const string Path = "/api/get-data";

    [Test, MoqAutoData]
    public void Then_Next_And_Prev_Links_Are_Returned_When_Not_First_Nor_Last_Page(
        PagedQuery request,
        GetApprenticeshipsByDatesResponse response,
        Mock<HttpContext> httpContext,
        [Frozen] Mock<IHttpContextAccessor> mockHttpContextAccessor,
        PagedLinkHeaderService sut
    )
    {
        response.TotalItems = 100;
        response.Page = 3;
        response.PageSize = 10;
        request.PageSize = 10;
        request.Page = 2;

        var httpRequest = new Mock<HttpRequest>();
        httpRequest.Setup(x => x.Method).Returns("GET");
        httpRequest.Setup(x => x.Scheme).Returns(Schema);
        httpRequest.Setup(x => x.Host).Returns(new HostString(Domain));
        httpRequest.Setup(x => x.Path).Returns(Path);
        httpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues>
        {
            { "start", "2025-01-01" },
            { "end", "2026-01-01" },
        }));

        httpContext.Setup(m => m.Request).Returns(httpRequest.Object);

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var links = sut.GetPageLinks(request, response);
        links.Should().NotBeNull();

        var prevLink = links.Value.First().Split(",").FirstOrDefault(x => x.Contains("rel=\"prev\""));
        var nextLink = links.Value.First().Split(",").FirstOrDefault(x => x.Contains("rel=\"next\""));

        prevLink.Should().NotBeNull();
        nextLink.Should().NotBeNull();

        prevLink.Split(";").First().Should().Be($"{Schema}://{Domain}{Path}?start=2025-01-01&end=2026-01-01&page={request.Page - 1}&pageSize={response.PageSize}");
        nextLink.Split(";").First().Should().Be($"{Schema}://{Domain}{Path}?start=2025-01-01&end=2026-01-01&page={request.Page + 1}&pageSize={response.PageSize}");
    }
}
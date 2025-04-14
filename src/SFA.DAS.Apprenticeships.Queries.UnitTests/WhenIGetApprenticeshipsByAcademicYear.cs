using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;
using SFA.DAS.Apprenticeships.InnerApi.Responses;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsByAcademicYear;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;

public class WhenIGetApprenticeshipsByAcademicYear
{
    private Fixture _fixture;
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepository;
    private Mock<IApprenticeshipsOuterApiClient> _apiClient;
    private GetApprenticeshipsByAcademicYearQueryHandler _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _apprenticeshipQueryRepository = new Mock<IApprenticeshipQueryRepository>();
        _apiClient = new Mock<IApprenticeshipsOuterApiClient>();
        _sut = new GetApprenticeshipsByAcademicYearQueryHandler(_apprenticeshipQueryRepository.Object);
    }

    [Test]
    public async Task ThenApprenticeshipsAreReturned()
    {
        //Arrange
        var queryResult = _fixture.Create<PagedResult<Apprenticeship>>();
        var expectedResult = new GetApprenticeshipsByAcademicYearResponse
        {
            Items = queryResult.Data.Select(x => new GetApprenticeshipsByDatesResponseItem
            {
                Uln = x.Uln
            }),
            Page = queryResult.Page,
            PageSize = queryResult.PageSize,
            TotalItems = queryResult.TotalItems
        };


        const int academicYear = 2526;
        const int pageSize = 20;
        const int pageNumber = 1;

        var query = new GetApprenticeshipsByAcademicYearRequest(1000, academicYear, pageNumber, pageSize);

        var dates = AcademicYearParser.ParseFrom(academicYear);

        _apprenticeshipQueryRepository
            .Setup(x => x.GetByDates(query.UkPrn, dates, pageNumber, pageSize, pageSize, 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();
        
        //Act
        var actualResult = await _sut.Handle(query);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);

        _apprenticeshipQueryRepository.Verify();
        _apiClient.Verify();
    }
}
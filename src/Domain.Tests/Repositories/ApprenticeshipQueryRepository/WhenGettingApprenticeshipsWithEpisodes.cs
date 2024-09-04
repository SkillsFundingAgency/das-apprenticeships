using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository;

public class WhenGettingApprenticeshipsWithEpisodes
{
    private Domain.Repositories.ApprenticeshipQueryRepository _sut = null!;
    private Fixture _fixture = null!;
    private ApprenticeshipsDataContext _dbContext = null!;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();
    }

    [TearDown]
    public void CleanUp()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task ThenReturnEmptyListWhenNoApprenticeshipsFoundForUkprn()
    {
        //Arrange
        SetUpApprenticeshipQueryRepository();

        //Act
        var result = await _sut.GetApprenticeshipsWithEpisodes(_fixture.Create<long>());

        //Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task ThenTheCorrectDataIsReturned()
    {
        //Arrange
        SetUpApprenticeshipQueryRepository();

        var apprenticeshipKey = _fixture.Create<Guid>();
        var episode1Key = _fixture.Create<Guid>();
        var episode2Key = _fixture.Create<Guid>();
        var price1Key = _fixture.Create<Guid>();
        var price2Key = _fixture.Create<Guid>();
        var price3Key = _fixture.Create<Guid>();
        var price4Key = _fixture.Create<Guid>();
        var ukprn = _fixture.Create<long>();
        var startDate = _fixture.Create<DateTime>();
        var ageAtStartOfApprenticeship = 20;
        var dob = startDate.AddYears(-20).AddMonths(-6);
        var endDate = startDate.AddYears(2);
        var uln = _fixture.Create<long>().ToString();
        var trainingCode = _fixture.Create<string>();

        var episodePrice1 = _fixture.Build<EpisodePrice>()
            .With(x => x.Key, price1Key)
            .With(x => x.EpisodeKey, episode1Key)
            .With(x => x.StartDate, startDate)
            .With(x => x.EndDate, startDate.AddDays(1))
            .With(x => x.IsDeleted, false)
            .Create();
        var episodePrice2 = _fixture.Build<EpisodePrice>()
            .With(x => x.Key, price2Key)
            .With(x => x.EpisodeKey, episode1Key)
            .With(x => x.StartDate, startDate.AddDays(1))
            .With(x => x.EndDate, startDate.AddMonths(8))
            .With(x => x.IsDeleted, false)
            .Create();

        var episode1 = _fixture.Build<Episode>()
            .With(x => x.Key, episode1Key)
            .With(x => x.Prices, new List<EpisodePrice> { episodePrice1, episodePrice2 })
            .With(x => x.Ukprn, ukprn)
            .With(x => x.TrainingCode, trainingCode)
            .Create();

        var episodePrice3 = _fixture.Build<EpisodePrice>()
            .With(x => x.Key, price3Key)
            .With(x => x.EpisodeKey, episode2Key)
            .With(x => x.StartDate, startDate.AddMonths(8))
            .With(x => x.EndDate, startDate.AddYears(1))
            .With(x => x.IsDeleted, false)
            .Create();
        var episodePrice4 = _fixture.Build<EpisodePrice>()
            .With(x => x.Key, price4Key)
            .With(x => x.EpisodeKey, episode2Key)
            .With(x => x.StartDate, startDate.AddYears(1))
            .With(x => x.EndDate, endDate)
            .With(x => x.IsDeleted, false)
            .Create();

        var episode2 = _fixture.Build<Episode>()
            .With(x => x.Key, episode2Key)
            .With(x => x.Prices, new List<EpisodePrice> { episodePrice3, episodePrice4 })
            .With(x => x.Ukprn, ukprn)
            .With(x => x.TrainingCode, trainingCode)
            .Create();

        var apprenticeships = new[]
        {
            _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                .With(x => x.Key, apprenticeshipKey)
                .With(x => x.Episodes, new List<Episode>() { episode1, episode2 })
                .With(x => x.DateOfBirth, dob)
                .With(x => x.Uln, uln)
                .Create(),
        };

        await _dbContext.AddRangeAsync(apprenticeships);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetApprenticeshipsWithEpisodes(ukprn);

        // Assert
        result.Should().NotBeNull();
        var apprenticeship = result.SingleOrDefault();
        apprenticeship.Should().NotBeNull();
        apprenticeship.StartDate.Should().Be(startDate);
        apprenticeship.AgeAtStartOfApprenticeship.Should().Be(ageAtStartOfApprenticeship);
        apprenticeship.Key.Should().Be(apprenticeshipKey);
        apprenticeship.PlannedEndDate.Should().Be(endDate);
        apprenticeship.Uln.Should().Be(uln);
        apprenticeship.Episodes.Count.Should().Be(2);

        var resultEpisode1 = apprenticeship.Episodes.SingleOrDefault(x => x.Key == episode1Key);
        resultEpisode1.Should().NotBeNull();
        resultEpisode1.TrainingCode.Should().Be(trainingCode);
        resultEpisode1.Prices.Count.Should().Be(2);

        resultEpisode1.Prices.Should().Contain(x =>
            x.EndDate == episodePrice1.EndDate
            && x.EndPointAssessmentPrice == episodePrice1.EndPointAssessmentPrice
            && x.FundingBandMaximum == episodePrice1.FundingBandMaximum
            && x.StartDate == episodePrice1.StartDate
            && x.TotalPrice == episodePrice1.TotalPrice
            && x.TrainingPrice == episodePrice1.TrainingPrice);

        resultEpisode1.Prices.Should().Contain(x =>
            x.EndDate == episodePrice2.EndDate
            && x.EndPointAssessmentPrice == episodePrice2.EndPointAssessmentPrice
            && x.FundingBandMaximum == episodePrice2.FundingBandMaximum
            && x.StartDate == episodePrice2.StartDate
            && x.TotalPrice == episodePrice2.TotalPrice
            && x.TrainingPrice == episodePrice2.TrainingPrice);

        var resultEpisode2 = apprenticeship.Episodes.SingleOrDefault(x => x.Key == episode2Key);
        resultEpisode2.Should().NotBeNull();
        resultEpisode2.TrainingCode.Should().Be(trainingCode);
        resultEpisode2.Prices.Count.Should().Be(2);

        resultEpisode2.Prices.Should().Contain(x =>
            x.EndDate == episodePrice3.EndDate
            && x.EndPointAssessmentPrice == episodePrice3.EndPointAssessmentPrice
            && x.FundingBandMaximum == episodePrice3.FundingBandMaximum
            && x.StartDate == episodePrice3.StartDate
            && x.TotalPrice == episodePrice3.TotalPrice
            && x.TrainingPrice == episodePrice3.TrainingPrice);

        resultEpisode2.Prices.Should().Contain(x =>
            x.EndDate == episodePrice4.EndDate
            && x.EndPointAssessmentPrice == episodePrice4.EndPointAssessmentPrice
            && x.FundingBandMaximum == episodePrice4.FundingBandMaximum
            && x.StartDate == episodePrice4.StartDate
            && x.TotalPrice == episodePrice4.TotalPrice
            && x.TrainingPrice == episodePrice4.TrainingPrice);
    }

    private void SetUpApprenticeshipQueryRepository()
    {
        _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
        var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
        _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
    }
}
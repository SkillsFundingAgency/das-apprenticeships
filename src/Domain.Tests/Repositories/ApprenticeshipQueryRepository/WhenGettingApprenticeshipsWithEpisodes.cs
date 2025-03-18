﻿using System;
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
using SFA.DAS.Apprenticeships.DataAccess.Extensions;
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

        var ukprn = _fixture.Create<long>();
        var startDate = _fixture.Create<DateTime>();
        var ageAtStartOfApprenticeship = 20;
        var endDate = startDate.AddYears(2);
        var trainingCode = _fixture.Create<string>();

        var episodePrice1 = CreateEpisodePrice(episode1Key, startDate, startDate.AddDays(1));
        var episodePrice2 = CreateEpisodePrice(episode1Key, startDate.AddDays(1), startDate.AddMonths(8));
        var episode1 = CreateEpisode(episode1Key, ukprn, trainingCode, episodePrice1, episodePrice2);

        var episodePrice3 = CreateEpisodePrice(episode2Key, startDate.AddMonths(8), startDate.AddYears(1));
        var episodePrice4 = CreateEpisodePrice(episode2Key, startDate.AddYears(1), endDate);
        var episode2 = CreateEpisode(episode2Key, ukprn, trainingCode, episodePrice3, episodePrice4);

        var apprenticeshipRecord = _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                .With(x => x.Key, apprenticeshipKey)
                .With(x => x.Episodes, new List<Episode>() { episode1, episode2 })
                .With(x => x.DateOfBirth, startDate.AddYears(-20).AddMonths(-6))
                .With(x => x.Uln, _fixture.Create<long>().ToString())
                .With(x => x.WithdrawalRequests, new List<WithdrawalRequest>())
                .Create();

        await _dbContext.AddRangeAsync(new[] { apprenticeshipRecord });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetApprenticeshipsWithEpisodes(ukprn);

        // Assert
        result.Should().NotBeNull();
        var apprenticeship = result.SingleOrDefault();
        AssertApprenticeship(apprenticeshipRecord, startDate, endDate, ageAtStartOfApprenticeship, apprenticeship);

        var resultEpisode1 = apprenticeship.Episodes.SingleOrDefault(x => x.Key == episode1Key);
        AssertEpisode(episode1, resultEpisode1);
        resultEpisode1.Prices.Should().Contain(x => AssertPrice(episodePrice1, x));
        resultEpisode1.Prices.Should().Contain(x => AssertPrice(episodePrice2, x));


        var resultEpisode2 = apprenticeship.Episodes.SingleOrDefault(x => x.Key == episode2Key);
        AssertEpisode(episode2, resultEpisode2);
        resultEpisode2.Prices.Should().Contain(x => AssertPrice(episodePrice3, x));
        resultEpisode2.Prices.Should().Contain(x => AssertPrice(episodePrice4, x));
    }

    [Test]
    public async Task ThenWithdrawnDataReturnedWhenWithdrawRequestExists()
    {
        //Arrange
        SetUpApprenticeshipQueryRepository();

        var apprenticeshipKey = _fixture.Create<Guid>();
        var episodeKey = _fixture.Create<Guid>();

        var ukprn = _fixture.Create<long>();
        var startDate = _fixture.Create<DateTime>();
        var endDate = startDate.AddYears(2);
        var trainingCode = _fixture.Create<string>();

        var episodePrice = CreateEpisodePrice(episodeKey, startDate, endDate);
        var episode = CreateEpisode(episodeKey, ukprn, trainingCode, episodePrice);
        var withdrawRecord = _fixture.Build<WithdrawalRequest>()
            .With(x => x.ApprenticeshipKey, apprenticeshipKey)
            .With(x => x.LastDayOfLearning, startDate.AddYears(1))
            .Create();

        var apprenticeshipRecord = _fixture.Build<DataAccess.Entities.Apprenticeship.Apprenticeship>()
                .With(x => x.Key, apprenticeshipKey)
                .With(x => x.Episodes, new List<Episode>() { episode })
                .With(x => x.DateOfBirth, startDate.AddYears(-20).AddMonths(-6))
                .With(x => x.Uln, _fixture.Create<long>().ToString())
                .With(x => x.WithdrawalRequests, new List<WithdrawalRequest>() { withdrawRecord })
                .Create();

        await _dbContext.AddRangeAsync(new[] { apprenticeshipRecord });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetApprenticeshipsWithEpisodes(ukprn);

        // Assert
        result.Should().NotBeNull();
        var apprenticeship = result.SingleOrDefault();
        apprenticeship.WithdrawnDate.Should().Be(withdrawRecord.LastDayOfLearning);
    }

    private void AssertApprenticeship(
        DataAccess.Entities.Apprenticeship.Apprenticeship expected,
        DateTime startDate,
        DateTime endDate,
        int age,
        DataTransferObjects.ApprenticeshipWithEpisodes actual)
    {
        actual.Should().NotBeNull();
        actual.StartDate.Should().Be(startDate);
        actual.AgeAtStartOfApprenticeship.Should().Be(age);
        actual.Key.Should().Be(expected.Key);
        actual.PlannedEndDate.Should().Be(endDate);
        actual.Uln.Should().Be(expected.Uln);
        actual.Episodes.Count.Should().Be(expected.Episodes.Count);
    }

    private void AssertEpisode(Episode expected, DataTransferObjects.Episode actual)
    {
        actual.Should().NotBeNull();
        actual.TrainingCode.Should().Be(expected.TrainingCode);
        actual.Prices.Count.Should().Be(expected.Prices.Count);
    }

    private bool AssertPrice(EpisodePrice expected, DataTransferObjects.EpisodePrice actual)
    {
        return actual.EndDate == expected.EndDate
            && actual.EndPointAssessmentPrice == expected.EndPointAssessmentPrice
            && actual.FundingBandMaximum == expected.FundingBandMaximum
            && actual.StartDate == expected.StartDate
            && actual.TotalPrice == expected.TotalPrice
            && actual.TrainingPrice == expected.TrainingPrice;
    }

    private void SetUpApprenticeshipQueryRepository()
    {
        _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
        var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
        _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
    }

    private EpisodePrice CreateEpisodePrice(Guid episodeKey, DateTime start, DateTime end)
    {
        return _fixture.Build<EpisodePrice>()
            .With(x => x.Key, _fixture.Create<Guid>())
            .With(x => x.EpisodeKey, episodeKey)
            .With(x => x.StartDate, start)
            .With(x => x.EndDate, end)
            .With(x => x.IsDeleted, false)
            .Create();
    }

    private Episode CreateEpisode(Guid key, long ukprn, string trainingCode, params EpisodePrice[] prices)
    {
        return _fixture.Build<Episode>()
            .With(x => x.Key, key)
            .With(x => x.Prices, prices.ToList())
            .With(x => x.Ukprn, ukprn)
            .With(x => x.TrainingCode, trainingCode)
            .Create();
    }
}
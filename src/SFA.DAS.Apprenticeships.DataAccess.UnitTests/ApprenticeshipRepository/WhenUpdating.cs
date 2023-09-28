using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.DataAccess.UnitTests.ApprenticeshipRepository;

public class WhenUpdating
{
    private Repositories.ApprenticeshipRepository _sut;
    private Fixture _fixture;
    private ApprenticeshipsDataContext _dbContext;
    private Mock<IDomainEventDispatcher> _domainEventDispatcher;
    private Mock<IApprenticeshipFactory> _apprenticeshipFactory;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();
        _fixture.Customize(new ApprenticeshipCustomization());

        var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>().UseInMemoryDatabase("EmployerIncentivesDbContext" + Guid.NewGuid()).Options;
        _dbContext = new ApprenticeshipsDataContext(options);

        _domainEventDispatcher = new Mock<IDomainEventDispatcher>();
        _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();

        _sut = new Repositories.ApprenticeshipRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), _domainEventDispatcher.Object, _apprenticeshipFactory.Object);

    }

    [TearDown]
    public void CleanUp()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Then_the_apprenticeship_price_history_is_added_to_the_data_store()
    {
        // Arrange
        var testApprenticeship = _fixture.Create<Apprenticeship>();
        await _sut.Add(testApprenticeship);
        testApprenticeship.AddPriceChange(DateTime.Now, 1, 2, DateTime.Now);
           
        // Act
        await _sut.Update(testApprenticeship);
        var storedApprenticeship = _dbContext.Apprenticeships.Include(a => a.PriceHistory).Single();

        // Assert
        var expected = testApprenticeship.PriceHistory.ToArray();
        var actual = storedApprenticeship.PriceHistory.ToArray();

        actual.Should().HaveCount(1);
        actual[0].ApprenticeshipKey.Should().Be(testApprenticeship.Key);
        actual[0].AssessmentPrice.Should().Be(expected[0].AssessmentPrice);
        actual[0].ApprovedDate.Should().Be(expected[0].ApprovedDate);
        actual[0].ApprovedDate.Should().Be(expected[0].ApprovedDate);
        actual[0].EffectiveFrom.Should().Be(expected[0].EffectiveFrom);
        actual[0].TrainingPrice.Should().Be(expected[0].TrainingPrice);
        actual[0].TotalPrice.Should().Be(expected[0].TotalPrice);
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Infrastructure.UnitTests
{
    public class WhenGettingFundingBandMaximum
    {
        private Mock<IApprenticeshipsOuterApiClient> _apprenticeshipsOuterApiClient;
        private FundingBandMaximumService _service;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _apprenticeshipsOuterApiClient = new Mock<IApprenticeshipsOuterApiClient>();
            _service = new FundingBandMaximumService(_apprenticeshipsOuterApiClient.Object);
        }

        [Test]
        public async Task ThenValueFromApiIsReturned()
        {
            var courseCode = _fixture.Create<int>();
            var getStandardResponse = _fixture.Create<GetStandardResponse>();
            _apprenticeshipsOuterApiClient.Setup(x => x.GetStandard(courseCode)).ReturnsAsync(getStandardResponse);
            var result = await _service.GetFundingBandMaximum(courseCode, null);
            result.Should().Be(getStandardResponse.MaxFunding);
        }

        [Test]
        public async Task ThenCorrectValueFromApiForPreviousFundingBandMaximumIsReturned()
        {
            var courseCode = _fixture.Create<int>();
            var getStandardResponse = _fixture.Create<GetStandardResponse>();
            getStandardResponse.ApprenticeshipFunding = _fixture.CreateMany<GetStandardFundingResponse>(2).ToList();
            getStandardResponse.ApprenticeshipFunding[0].EffectiveFrom = new DateTime(2022, 01, 01);
            getStandardResponse.ApprenticeshipFunding[0].EffectiveTo = new DateTime(2022, 05, 05);
            getStandardResponse.ApprenticeshipFunding[1].EffectiveFrom = new DateTime(2022, 05, 06);
            getStandardResponse.ApprenticeshipFunding[1].EffectiveTo = null;
            _apprenticeshipsOuterApiClient.Setup(x => x.GetStandard(courseCode)).ReturnsAsync(getStandardResponse);
            var result = await _service.GetFundingBandMaximum(courseCode, new DateTime(2022, 01, 01));
            result.Should().Be(getStandardResponse.ApprenticeshipFunding[0].MaxEmployerLevyCap);
        }

        [Test]
        public async Task ThenCorrectValueFromApiForMostRecentFundingBandMaximumIsReturned()
        {
            var courseCode = _fixture.Create<int>();
            var getStandardResponse = _fixture.Create<GetStandardResponse>();
            getStandardResponse.ApprenticeshipFunding = _fixture.CreateMany<GetStandardFundingResponse>(2).ToList();
            getStandardResponse.ApprenticeshipFunding[0].EffectiveFrom = new DateTime(2022, 01, 01);
            getStandardResponse.ApprenticeshipFunding[0].EffectiveTo = new DateTime(2022, 05, 05);
            getStandardResponse.ApprenticeshipFunding[1].EffectiveFrom = new DateTime(2022, 05, 06);
            getStandardResponse.ApprenticeshipFunding[1].EffectiveTo = null;
            _apprenticeshipsOuterApiClient.Setup(x => x.GetStandard(courseCode)).ReturnsAsync(getStandardResponse);
            var result = await _service.GetFundingBandMaximum(courseCode, new DateTime(2022, 05, 07));
            result.Should().Be(getStandardResponse.ApprenticeshipFunding[1].MaxEmployerLevyCap);
        }
    }
}
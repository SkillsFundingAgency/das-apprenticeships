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
        private FundingBandMaximumService _adapter;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _apprenticeshipsOuterApiClient = new Mock<IApprenticeshipsOuterApiClient>();
            _adapter = new FundingBandMaximumService(_apprenticeshipsOuterApiClient.Object);
        }

        [Test]
        public async Task ThenValueFromApiIsReturned()
        {
            var courseCode = _fixture.Create<int>();
            var getStandardResponse = _fixture.Create<GetStandardResponse>();
            _apprenticeshipsOuterApiClient.Setup(x => x.GetStandard(courseCode)).ReturnsAsync(getStandardResponse);
            var result = await _adapter.GetFundingBandMaximum(courseCode);
            result.Should().Be(getStandardResponse.MaxFunding);
        }
    }
}
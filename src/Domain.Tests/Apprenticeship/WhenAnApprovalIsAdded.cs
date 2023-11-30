using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenAnApprovalIsAdded
    {
        private Domain.Apprenticeship.ApprenticeshipDomainModel _apprenticeship;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            var apprenticeshipFactory = new ApprenticeshipFactory();
            _fixture = new Fixture();
            _apprenticeship = apprenticeshipFactory.CreateNew("1234435", "TRN", new DateTime(2000, 10, 16), "Ron", "Swanson", _fixture.Create<decimal?>(), _fixture.Create<decimal?>(), _fixture.Create<decimal>(), _fixture.Create<string>(), _fixture.Create<int>());
        }

        [Test]
        public void ThenTheApprovalIsAdded()
        {
            var expectedModel = _fixture.Create<ApprovalModel>();
            _apprenticeship.AddApproval(expectedModel.ApprovalsApprenticeshipId, expectedModel.UKPRN, expectedModel.EmployerAccountId, expectedModel.LegalEntityName, expectedModel.ActualStartDate, expectedModel.PlannedEndDate, expectedModel.AgreedPrice, expectedModel.FundingEmployerAccountId, expectedModel.FundingType, expectedModel.FundingBandMaximum, expectedModel.PlannedStartDate, expectedModel.FundingPlatform);

            var approval = _apprenticeship.GetEntity().Approvals.Single();

            approval.Should().BeEquivalentTo(expectedModel, opts => opts.Excluding(x => x.Id));
        }
    }
}

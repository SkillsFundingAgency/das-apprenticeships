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
        private Domain.Apprenticeship.Apprenticeship _apprenticeship;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            var apprenticeshipFactory = new ApprenticeshipFactory();
            _apprenticeship = apprenticeshipFactory.CreateNew("1234435", "TRN", DateTime.Now);

            _fixture = new Fixture();
        }

        [Test]
        public void ThenTheApprovalIsAdded()
        {
            var expectedModel = _fixture.Create<ApprovalModel>();
            _apprenticeship.AddApproval(expectedModel.ApprovalsApprenticeshipId, expectedModel.UKPRN, expectedModel.EmployerAccountId, expectedModel.LegalEntityName, expectedModel.ActualStartDate, expectedModel.PlannedEndDate, expectedModel.AgreedPrice, expectedModel.FundingEmployerAccountId, expectedModel.FundingType, expectedModel.FundingBandMaximum);

            var approval = _apprenticeship.GetModel().Approvals.Single();

            approval.Should().BeEquivalentTo(expectedModel, opts => opts.Excluding(x => x.Id));
        }
    }
}

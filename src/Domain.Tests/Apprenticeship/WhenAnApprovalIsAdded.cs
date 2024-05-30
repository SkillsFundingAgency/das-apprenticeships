using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Apprenticeship
{
    [TestFixture]
    public class WhenAnApprovalIsAdded
    {
        private ApprenticeshipDomainModel _apprenticeship;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void ThenTheApprovalIsAdded()
        {
            var apprenticeship = CreateNewApprenticeship();
            var approvalDomainModel = ApprovalDomainModel.Get(_fixture.Create<Approval>());
            
            apprenticeship.AddApproval(approvalDomainModel.ApprovalsApprenticeshipId, approvalDomainModel.LegalEntityName, approvalDomainModel.ActualStartDate, approvalDomainModel.PlannedEndDate, approvalDomainModel.AgreedPrice, approvalDomainModel.FundingEmployerAccountId, approvalDomainModel.FundingType, approvalDomainModel.FundingBandMaximum, approvalDomainModel.PlannedStartDate, approvalDomainModel.FundingPlatform);

            var approval = apprenticeship.GetEntity().Approvals.Single();
            approval.Should().BeEquivalentTo(approvalDomainModel);
        }

        private ApprenticeshipDomainModel CreateNewApprenticeship()
        {
            var apprenticeshipFactory = new ApprenticeshipFactory();
            return apprenticeshipFactory.CreateNew(
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<DateTime>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<decimal?>(),
                _fixture.Create<decimal?>(),
                _fixture.Create<decimal>(),
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                _fixture.Create<DateTime>(),
                _fixture.Create<DateTime>(),
                _fixture.Create<long>(),
                _fixture.Create<long>(),
                _fixture.Create<long>(),
                _fixture.Create<string>());
        }
    }
}

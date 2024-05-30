using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command.AddApproval;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.TestHelpers.AutoFixture.Customizations;
using FluentAssertions;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Command.UnitTests.AddApproval
{
    [TestFixture]
    public class WhenAnApprovalIsAdded
    {
        private AddApprovalCommandHandler _commandHandler = null!;
        private Mock<IApprenticeshipFactory> _apprenticeshipFactory = null!;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository = null!;
        private Mock<IFundingBandMaximumService> _fundingBandMaximumService = null!;
        private Fixture _fixture = null!;

        [SetUp]
        public void SetUp()
        {
            _apprenticeshipFactory = new Mock<IApprenticeshipFactory>();
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _fundingBandMaximumService = new Mock<IFundingBandMaximumService>();
            _commandHandler = new AddApprovalCommandHandler(_apprenticeshipFactory.Object, _apprenticeshipRepository.Object, _fundingBandMaximumService.Object);

            _fixture = new Fixture();
            _fixture.Customize(new ApprenticeshipCustomization());
        }

        [Test]
        public async Task ThenTheApprovalIsCreated()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipFactory.Setup(x => x.CreateNew(
                    command.Uln,
                    command.TrainingCode,
                    command.DateOfBirth,
                    command.FirstName,
                    command.LastName,
                    command.TrainingPrice,
                    command.EndPointAssessmentPrice,
                    command.AgreedPrice,
                    command.ApprenticeshipHashedId,
                    (int)Math.Ceiling(command.AgreedPrice),
                    command.ActualStartDate,
                    command.PlannedEndDate,
                    command.AccountLegalEntityId,
                    command.UKPRN,
                    command.EmployerAccountId,
                    command.TrainingCourseVersion))
                .Returns(apprenticeship);
            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int)Math.Ceiling(command.AgreedPrice));

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().Approvals.Count == 1)));
        }

        [Test]
        public async Task ThenTheApprovalIsCreatedWithTheCorrectFundingBandMaximum()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var fundingBandMaximum = _fixture.Create<int>();

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync(fundingBandMaximum);

            _apprenticeshipFactory.Setup(x => x.CreateNew(
                    command.Uln,
                    command.TrainingCode,
                    command.DateOfBirth,
                    command.FirstName,
                    command.LastName,
                    command.TrainingPrice,
                    command.EndPointAssessmentPrice,
                    command.AgreedPrice,
                    command.ApprenticeshipHashedId,
                    fundingBandMaximum,
                    command.ActualStartDate,
                    command.PlannedEndDate,
                    command.AccountLegalEntityId,
                    command.UKPRN,
                    command.EmployerAccountId,
                    command.TrainingCourseVersion))
                .Returns(apprenticeship);

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().Approvals.Single().FundingBandMaximum == fundingBandMaximum)));
        }

        [Test]
        public async Task ThenExceptionIsThrownWhenNoFundingBandMaximumForTheGivenDateAndCourseCodeIsPresent()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int?)null);

            await _commandHandler.Invoking(x => x.Handle(command, It.IsAny<CancellationToken>())).Should()
                .ThrowAsync<Exception>()
                .WithMessage(
                    $"No funding band maximum found for course {command.TrainingCode} for given date {command.ActualStartDate?.ToString("u")}. Approvals Apprenticeship Id: {command.ApprovalsApprenticeshipId}");
        }

        [Test]
        public async Task ThenTheApprovalIsCreatedWithNullPlannedStartDateWhenOneIsNotSpecified()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            command.PlannedStartDate = new DateTime();

            _apprenticeshipFactory.Setup(x => x.CreateNew(
                    command.Uln, 
                    command.TrainingCode, 
                    command.DateOfBirth, 
                    command.FirstName, 
                    command.LastName, 
                    command.TrainingPrice, 
                    command.EndPointAssessmentPrice, 
                    command.AgreedPrice, 
                    command.ApprenticeshipHashedId, 
                    (int)Math.Ceiling(command.AgreedPrice), 
                    command.ActualStartDate, 
                    command.PlannedEndDate, 
                    command.AccountLegalEntityId, 
                    command.UKPRN, 
                    command.EmployerAccountId,
                    command.TrainingCourseVersion))
                .Returns(apprenticeship);
            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int)Math.Ceiling(command.AgreedPrice));

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().Approvals.Single().PlannedStartDate == null)));
        }

        [Test]
        public async Task ThenTheApprovalIsCreatedWithPlannedStartDateWhenOneIsSpecified()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();

            _apprenticeshipFactory.Setup(x => x.CreateNew(
                    command.Uln,
                    command.TrainingCode,
                    command.DateOfBirth,
                    command.FirstName,
                    command.LastName,
                    command.TrainingPrice,
                    command.EndPointAssessmentPrice,
                    command.AgreedPrice,
                    command.ApprenticeshipHashedId,
                    (int)Math.Ceiling(command.AgreedPrice),
                    command.ActualStartDate,
                    command.PlannedEndDate,
                    command.AccountLegalEntityId,
                    command.UKPRN,
                    command.EmployerAccountId,
                    command.TrainingCourseVersion))
                .Returns(apprenticeship);
            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int)Math.Ceiling(command.AgreedPrice));

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<ApprenticeshipDomainModel>(y => y.GetEntity().Approvals.Single().PlannedStartDate == command.PlannedStartDate)));
        }

        [Test]
        public async Task ThenCorrectDateIsUsedWhenGettingFundingBandMaximumForApprenticeshipWithDasFundingPlatform()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var fundingBandMaximum = _fixture.Create<int>();
            command.FundingPlatform = FundingPlatform.DAS;

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync(fundingBandMaximum);

            _apprenticeshipFactory.Setup(x => x.CreateNew(
                    command.Uln,
                    command.TrainingCode,
                    command.DateOfBirth,
                    command.FirstName,
                    command.LastName,
                    command.TrainingPrice,
                    command.EndPointAssessmentPrice,
                    command.AgreedPrice,
                    command.ApprenticeshipHashedId,
                    fundingBandMaximum,
                    command.ActualStartDate,
                    command.PlannedEndDate,
                    command.AccountLegalEntityId,
                    command.UKPRN,
                    command.EmployerAccountId,
                    command.TrainingCourseVersion))
                .Returns(apprenticeship);

            await _commandHandler.Handle(command);

            _fundingBandMaximumService.Verify(x => x.GetFundingBandMaximum(trainingCodeInt, command.ActualStartDate));
        }

        [Test]
        public async Task ThenCorrectDateIsUsedWhenGettingFundingBandMaximumForApprenticeshipWithSldFundingPlatform()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<ApprenticeshipDomainModel>();
            var fundingBandMaximum = _fixture.Create<int>();
            command.FundingPlatform = FundingPlatform.SLD;

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync(fundingBandMaximum);

            _apprenticeshipFactory.Setup(x => x.CreateNew(
                    command.Uln,
                    command.TrainingCode,
                    command.DateOfBirth,
                    command.FirstName,
                    command.LastName,
                    command.TrainingPrice,
                    command.EndPointAssessmentPrice,
                    command.AgreedPrice,
                    command.ApprenticeshipHashedId,
                    fundingBandMaximum,
                    command.ActualStartDate,
                    command.PlannedEndDate, 
                    command.AccountLegalEntityId,
                    command.UKPRN,
                    command.EmployerAccountId,
                    command.TrainingCourseVersion))
                .Returns(apprenticeship);

            await _commandHandler.Handle(command);

            _fundingBandMaximumService.Verify(x => x.GetFundingBandMaximum(trainingCodeInt, command.PlannedStartDate));
        }
    }
}

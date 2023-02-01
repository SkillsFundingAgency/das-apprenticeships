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
        private AddApprovalCommandHandler _commandHandler;
        private Mock<IApprenticeshipFactory> _apprenticeshipFactory;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;
        private Mock<IFundingBandMaximumService> _fundingBandMaximumService;
        private Fixture _fixture;

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
            var apprenticeship = _fixture.Create<Apprenticeship>();

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);
            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int)Math.Ceiling(command.AgreedPrice));

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<Apprenticeship>(y => y.GetModel().Approvals.Count == 1)));
        }

        [Test]
        public async Task ThenTheApprovalIsCreatedWithTheCorrectFundingBandMaximum()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<Apprenticeship>();
            var fundingBandMaximum = _fixture.Create<int>();

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync(fundingBandMaximum);

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<Apprenticeship>(y => y.GetModel().Approvals.Single().FundingBandMaximum == fundingBandMaximum)));
        }

        [Test]
        public async Task ThenExceptionIsThrownWhenNoFundingBandMaximumForTheGivenDateAndCourseCodeIsPresent()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<Apprenticeship>();

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int?)null);

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);

            await _commandHandler.Invoking(x => x.Handle(command, It.IsAny<CancellationToken>())).Should()
                .ThrowAsync<Exception>()
                .WithMessage(
                    $"No funding band maximum found for course {command.TrainingCode} for given date {command.ActualStartDate?.ToString("u")}. Apprenticeship Key: {apprenticeship.Key}");
        }

        [Test]
        public async Task ThenTheApprovalIsCreatedWithNullPlannedStartDateWhenOneIsNotSpecified()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<Apprenticeship>();
            command.PlannedStartDate = new DateTime();

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);
            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int)Math.Ceiling(command.AgreedPrice));

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<Apprenticeship>(y => y.GetModel().Approvals.Single().PlannedStartDate == null)));
        }

        [Test]
        public async Task ThenTheApprovalIsCreatedWithPlannedStartDateWhenOneIsSpecified()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<Apprenticeship>();

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);
            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync((int)Math.Ceiling(command.AgreedPrice));

            await _commandHandler.Handle(command);

            _apprenticeshipRepository.Verify(x => x.Add(It.Is<Apprenticeship>(y => y.GetModel().Approvals.Single().PlannedStartDate == command.PlannedStartDate)));
        }

        [Test]
        public async Task ThenCorrectDateIsUsedWhenGettingFundingBandMaximumForApprenticeshipWithDasFundingPlatform()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<Apprenticeship>();
            var fundingBandMaximum = _fixture.Create<int>();
            command.FundingPlatform = FundingPlatform.DAS;

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync(fundingBandMaximum);

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);

            await _commandHandler.Handle(command);

            _fundingBandMaximumService.Verify(x => x.GetFundingBandMaximum(trainingCodeInt, command.ActualStartDate));
        }

        [Test]
        public async Task ThenCorrectDateIsUsedWhenGettingFundingBandMaximumForApprenticeshipWithSldFundingPlatform()
        {
            var command = _fixture.Create<AddApprovalCommand>();
            var trainingCodeInt = _fixture.Create<int>();
            command.TrainingCode = trainingCodeInt.ToString();
            var apprenticeship = _fixture.Create<Apprenticeship>();
            var fundingBandMaximum = _fixture.Create<int>();
            command.FundingPlatform = FundingPlatform.SLD;

            _fundingBandMaximumService.Setup(x => x.GetFundingBandMaximum(trainingCodeInt, It.IsAny<DateTime?>()))
                .ReturnsAsync(fundingBandMaximum);

            _apprenticeshipFactory.Setup(x => x.CreateNew(command.Uln, command.TrainingCode, command.DateOfBirth)).Returns(apprenticeship);

            await _commandHandler.Handle(command);

            _fundingBandMaximumService.Verify(x => x.GetFundingBandMaximum(trainingCodeInt, command.PlannedStartDate));
        }
    }
}

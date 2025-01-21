using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SFA.DAS.Apprenticeships.Functions
{
    public class HandleApprovalCreated
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public HandleApprovalCreated(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [FunctionName("HandleApprovalCreatedEvent")]
        public async Task HandleCommand([NServiceBusTrigger(Endpoint = QueueNames.ApprovalCreated)] ApprenticeshipCreatedEvent @event,
            ILogger log)
        {
            if (!@event.IsOnFlexiPaymentPilot.GetValueOrDefault())
            {
                log.LogInformation("Apprenticeship {hashedId} is not funded by DAS and therefore, no further action will be taken.", @event.ApprenticeshipHashedId);
                return;
            }
            await _commandDispatcher.Send(new AddApprenticeshipCommand
            {
                TrainingCode = @event.TrainingCode,
                ActualStartDate = @event.ActualStartDate,
                TotalPrice = @event.PriceEpisodes[0].Cost,
                TrainingPrice = @event.PriceEpisodes[0].TrainingPrice,
                EndPointAssessmentPrice = @event.PriceEpisodes[0].EndPointAssessmentPrice,
                ApprovalsApprenticeshipId = @event.ApprenticeshipId,
                EmployerAccountId = @event.AccountId,
                FundingEmployerAccountId = @event.TransferSenderId,
                FundingType = GetFundingType(@event),
                LegalEntityName = @event.LegalEntityName,
                PlannedEndDate = @event.EndDate,
                UKPRN = @event.ProviderId,
                Uln = @event.Uln,
                DateOfBirth = @event.DateOfBirth,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                ApprenticeshipHashedId = @event.ApprenticeshipHashedId,
                FundingPlatform = @event.IsOnFlexiPaymentPilot.HasValue ? (@event.IsOnFlexiPaymentPilot.Value ? FundingPlatform.DAS : FundingPlatform.SLD) : null,
                AccountLegalEntityId = @event.AccountLegalEntityId,
                TrainingCourseVersion = @event.TrainingCourseVersion
            });
        }

        private FundingType GetFundingType(ApprenticeshipCreatedEvent @event)
        {
            if (@event.TransferSenderId.HasValue)
            {
                return FundingType.Transfer;
            }

            if (@event.ApprenticeshipEmployerTypeOnApproval == ApprenticeshipEmployerType.NonLevy)
            {
                return FundingType.NonLevy;
            }

            return FundingType.Levy;
        }
    }
}
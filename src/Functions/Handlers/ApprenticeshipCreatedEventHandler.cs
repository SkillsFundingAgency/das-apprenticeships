using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.AddApprenticeship;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.Apprenticeships.Functions.Handlers
{
    public class ApprenticeshipCreatedEventHandler(ICommandDispatcher commandDispatcher, ILogger<ApprenticeshipCreatedEventHandler> logger) : IHandleMessages<ApprenticeshipCreatedEvent>
    {
        public async Task Handle(ApprenticeshipCreatedEvent @event, IMessageHandlerContext context)
        {
            logger.LogInformation("Handling ApprenticeshipCreatedEvent");

            await commandDispatcher.Send(new AddApprenticeshipCommand
            {
                TrainingCode = @event.TrainingCode,
                ActualStartDate = @event.ActualStartDate ?? @event.StartDate,
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

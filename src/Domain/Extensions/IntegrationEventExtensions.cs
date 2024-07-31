using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.Extensions;

public static class IntegrationEventExtensions
{
    public static ApprenticeshipEpisode BuildEpisodeForIntegrationEvent(this ApprenticeshipDomainModel apprenticeship)
    {
        var latestEpisode = apprenticeship.LatestEpisode;
        var activePrices = latestEpisode.ActiveEpisodePrices;
        var prices = activePrices.Select(price => 
            new ApprenticeshipEpisodePrice
            {
                Key = price.Key,
                StartDate = price.StartDate,
                EndDate = price.EndDate,
                TrainingPrice = price.TrainingPrice,
                EndPointAssessmentPrice = price.EndPointAssessmentPrice,
                TotalPrice = price.TotalPrice,
                FundingBandMaximum = price.FundingBandMaximum
            }).ToList();

        return new ApprenticeshipEpisode()
        {
            Key = latestEpisode.Key,
            Ukprn = latestEpisode.Ukprn,
            EmployerAccountId = latestEpisode.EmployerAccountId,
            FundingType = latestEpisode.FundingType,
            FundingPlatform = latestEpisode.FundingPlatform,
            FundingEmployerAccountId = latestEpisode.FundingEmployerAccountId,
            LegalEntityName = latestEpisode.LegalEntityName,
            AccountLegalEntityId = latestEpisode.AccountLegalEntityId,
            AgeAtStartOfApprenticeship = apprenticeship.AgeAtStartOfApprenticeship,
            TrainingCode = latestEpisode.TrainingCode,
            TrainingCourseVersion = latestEpisode.TrainingCourseVersion,
            PaymentsFrozen = latestEpisode.PaymentsFrozen,
            Prices = prices
        };
    }
}

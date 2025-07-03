using SFA.DAS.Learning.Types;

namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipEpisode
{
    public Guid Key { get; set; }

    public long Ukprn { get; set; }

    public long EmployerAccountId { get; set; }

    public SFA.DAS.Apprenticeships.Enums.FundingType FundingType { get; set; }

    public SFA.DAS.Apprenticeships.Enums.FundingPlatform? FundingPlatform { get; set; }

    public long? FundingEmployerAccountId { get; set; }

    public string LegalEntityName { get; set; }

    public long? AccountLegalEntityId { get; set; }

    public int AgeAtStartOfApprenticeship { get; set; }

    public string TrainingCode { get; set; }

    public string? TrainingCourseVersion { get; set; }

    public bool PaymentsFrozen { get; set; }

    public List<ApprenticeshipEpisodePrice> Prices { get; set; }

    public static implicit operator ApprenticeshipEpisode(LearningEpisode learningEpisode)
    {
        return new ApprenticeshipEpisode
        {
            Key = learningEpisode.Key,
            Ukprn = learningEpisode.Ukprn,
            EmployerAccountId = learningEpisode.EmployerAccountId,
            FundingType = (Enums.FundingType) learningEpisode.FundingType,
            FundingPlatform = (Enums.FundingPlatform?) learningEpisode.FundingPlatform,
            FundingEmployerAccountId = learningEpisode.FundingEmployerAccountId,
            LegalEntityName = learningEpisode.LegalEntityName,
            AccountLegalEntityId = learningEpisode.AccountLegalEntityId,
            AgeAtStartOfApprenticeship = learningEpisode.AgeAtStartOfLearning,
            TrainingCode = learningEpisode.TrainingCode,
            TrainingCourseVersion = learningEpisode.TrainingCourseVersion,
            PaymentsFrozen = learningEpisode.PaymentsFrozen,
            Prices = learningEpisode.Prices?.Select(price => (ApprenticeshipEpisodePrice)price).ToList()
        };
    }
}
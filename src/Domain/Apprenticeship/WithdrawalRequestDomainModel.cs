using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship;

public class WithdrawalRequestDomainModel
{
    private readonly WithdrawalRequest _entity;

    public Guid Key => _entity.Key;
    public Guid ApprenticeshipKey => _entity.ApprenticeshipKey;
    public Guid EpisodeKey => _entity.EpisodeKey;
    public string Reason => _entity.Reason;
    public DateTime LastDayOfLearning => _entity.LastDayOfLearning;
    public DateTime CreatedDate => _entity.CreatedDate;
    public string? ProviderApprovedBy => _entity.ProviderApprovedBy;

    internal static WithdrawalRequestDomainModel New(
        Guid apprenticeshipKey, Guid episodeKey, string reason, DateTime lastDayOfLearning, DateTime createdDate, string providerApprovedBy)
    {
        return new WithdrawalRequestDomainModel(new WithdrawalRequest
        {
            ApprenticeshipKey = apprenticeshipKey,
            EpisodeKey = episodeKey,
            Reason = reason,
            LastDayOfLearning = lastDayOfLearning,
            CreatedDate = createdDate,
            ProviderApprovedBy = providerApprovedBy
        });
    }

    public WithdrawalRequestDomainModel(WithdrawalRequest entity) 
    {
        _entity = entity;
    }

    public WithdrawalRequest GetEntity()
    {
        return _entity;
    }
}
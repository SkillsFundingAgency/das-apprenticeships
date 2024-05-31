using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship;

public class FreezeRequestDomainModel
{
    private readonly FreezeRequest _entity;

    public Guid Key => _entity.Key;
    public Guid ApprenticeshipKey => _entity.ApprenticeshipKey;
    public string FrozenBy => _entity.FrozenBy;
    public DateTime FrozenDateTime => _entity.FrozenDateTime;
    public bool Unfrozen => _entity.Unfrozen;
    public DateTime? UnfrozenDateTime => _entity.UnfrozenDateTime;
    public string? UnfrozenBy => _entity.UnfrozenBy;

    internal static FreezeRequestDomainModel New(Guid apprenticeshipKey, string userId, DateTime frozenDateTime)
    {
        return new FreezeRequestDomainModel(new FreezeRequest
        {
            ApprenticeshipKey = apprenticeshipKey,
            FrozenBy = userId,
            FrozenDateTime = frozenDateTime,
            Unfrozen = false
        });
    }

    public FreezeRequest GetEntity()
    {
        return _entity;
    }

    internal static FreezeRequestDomainModel Get(FreezeRequest entity)
    {
        return new FreezeRequestDomainModel(entity);
    }

    private FreezeRequestDomainModel(FreezeRequest entity)
    {
        _entity = entity;
    }
}
